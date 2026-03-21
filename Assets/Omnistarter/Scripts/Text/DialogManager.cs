// author: Omnistudio
// version: 2026.03.21

using Omnis.Text.Conditions;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.Text
{
    public partial class DialogManager : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] protected ScriptableStyleSheet styleSheet;
        #endregion


        #region Fields

        public static DialogManager Instance;
        
        private readonly Dictionary<string, TextActor> actors = new();
        protected Dictionary<string, List<EntryBranch>> dialogScript;
        protected EntryBranch currBranch;
        protected TextActor currActor;
        private int currLineIndex;

        public DialogCommands commands;
        public Blackboard blackboard;
        
        #endregion


        #region Properties
        public ScriptableStyleSheet StyleSheet => styleSheet;
        public virtual int CurrLineIndex {
            get => currLineIndex;
            protected set {
                if (currBranch == null) {
                    currLineIndex = -1;
                } else {
                    // interrupt old commands
                    var oldLine = currBranch.actorLines[currLineIndex];
                    foreach (var cmd in oldLine.cmds) {
                        if (commands.TryGet(cmd.keyword, out var exe)) {
                            exe.OnInterrupted(this);
                        }
                    }
                    
                    if (value >= currBranch.actorLines.Count) {
                        FinishEntry();
                    } else {
                        currLineIndex = value;
                        var actorLine = currBranch.actorLines[value];
                        if (actors.TryGetValue(actorLine.actorId, out var actor)) {
                            actor.Line = actorLine.text;
                            currActor = actor;
                        }
                        // execute new commands
                        foreach (var cmd in actorLine.cmds) {
                            if (commands.TryGet(cmd.keyword, out var exe)) {
                                exe.OnExecute(cmd.args, this);
                            }
                        }
                    }
                }
            }
        }
        public TextActor CurrActor => currActor;
        #endregion


        #region Methods
        public virtual bool TryEnter(string entryName) {
            entryName = entryName.ToLowerInvariant();
            if (!dialogScript.TryGetValue(entryName, out var entry)) {
                Debug.LogWarning($"No entry named {entryName}.");
                return false;
            }

            foreach (var branch in entry) {
                if (branch.cond == null || branch.cond.Evaluate(blackboard)) {
                    currBranch = branch;
                    CurrLineIndex = 0;
                    return true;
                }
            }

            return false;
        }

        public virtual void NextLine() {
            CurrLineIndex++;
        }

        protected virtual void FinishEntry() {
            if (currBranch.nextEntry != null)
                TryEnter(currBranch.nextEntry);
        }


        public void AddActor(TextActor actor) => actors.Add(actor.actorId, actor);
        public void RemoveActor(TextActor actor) => actors.Remove(actor.actorId);
        #endregion


        #region Unity Methods
        protected virtual void Awake() {
            Instance = this;

            commands = new();

            // register any new commands at here
            commands.Register("add", new DialogCommandAdd());
            commands.Register("set", new DialogCommandSet());
            
            // or you can add components to this dialog manager
            var loadedCommands = GetComponents<IDialogCommand>();

            foreach (var cmd in loadedCommands) {
                commands.Register(cmd.Keyword, cmd);
            }
        }
        #endregion
    }



    public class EntryBranch
    {
        public List<ActorLine> actorLines = new();
        public ICondition cond = null;
        public string nextEntry = null;
    }

    public class ActorLine
    {
        public string actorId = null;
        public string text = null;
        public List<CommandInfo> cmds = new();
    }

    public struct CommandInfo
    {
        public string keyword;
        public string[] args;
    }
}
