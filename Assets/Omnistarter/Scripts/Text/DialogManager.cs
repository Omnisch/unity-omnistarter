// author: Omnistudio
// version: 2026.01.02

using Omnis.Text.Conditions;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.Text
{
    public partial class DialogManager : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private ScriptableStyleSheet styleSheet;
        #endregion


        #region Fields
        private readonly Dictionary<string, TextActor> actors = new();
        private Dictionary<string, List<EntryBranch>> dialogScript;
        private EntryBranch currBranch;

        public DialogCommands commands;
        public Blackboard blackboard;
        #endregion


        #region Properties
        public ScriptableStyleSheet StyleSheet => styleSheet;
        private int currLineIndex;
        protected int CurrLineIndex {
            get => currLineIndex;
            set {
                if (currBranch == null) {
                    currLineIndex = -1;
                } else {
                    if (value >= currBranch.actorLines.Count) {
                        FinishEntry();
                    } else {
                        currLineIndex = value;
                        var actorLine = currBranch.actorLines[value];
                        if (actors.TryGetValue(actorLine.actorId, out var actor))
                            actor.Line = actorLine.text;
                        // execute commands
                        foreach (var cmd in actorLine.cmds) {
                            if (commands.TryGet(cmd.keyword, out var exe)) {
                                StartCoroutine(exe.Execute(cmd.args, this));
                            }
                        }
                    }
                }
            }
        }
        public TextActor CurrActor => actors[currBranch.actorLines[currLineIndex].actorId];
        #endregion


        #region Methods
        public bool TryEnter(string entryName) {
            if (!dialogScript.ContainsKey(entryName)) {
                Debug.LogWarning($"No entry named {entryName}.");
                return false;
            }

            var entry = dialogScript[entryName];
            foreach (var branch in entry) {
                if (branch.cond.Evaluate(blackboard)) {
                    currBranch = branch;
                    CurrLineIndex = 0;
                    return true;
                }
            }

            return false;
        }

        public void NextLine(string callFromActor) {
            // If not from the active actor, then ignore it.
            if (currBranch != null && currBranch.actorLines[CurrLineIndex].actorId == callFromActor)
                CurrLineIndex++;
        }

        private void FinishEntry() {
            if (currBranch.nextEntry != "")
                TryEnter(currBranch.nextEntry);
        }


        public void AddActor(TextActor actor) => actors.Add(actor.actorId, actor);
        public void RemoveActor(TextActor actor) => actors.Remove(actor.actorId);
        #endregion


        #region Unity Methods
        private void Awake() {
            if (!EnsureSingleton())
                return;

            commands = new();

            // register any new commands at here
            commands.Register("add", new DialogCommandAdd());
            commands.Register("set", new DialogCommandSet());
        }
        #endregion
    }



    public class EntryBranch
    {
        public List<ActorLine> actorLines = new();
        public ICondition cond;
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
