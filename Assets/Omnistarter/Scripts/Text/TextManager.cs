// author: Omnistudio
// version: 2025.09.03

using System.Collections.Generic;
using UnityEngine;

namespace Omnis.Text
{
    public partial class TextManager : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private ScriptableStyleSheet styleSheet;
        #endregion

        #region Fields
        private readonly Dictionary<string, TextActor> actors = new();
        private Dictionary<string, List<EntryBranch>> dialogScript;
        public Dictionary<string, string> DialogState { get; private set; }
        private EntryBranch currBranch;
        #endregion

        #region Properties
        public ScriptableStyleSheet StyleSheet => this.styleSheet;
        private int currLineIndex;
        protected int CurrLineIndex
        {
            get => this.currLineIndex;
            set
            {
                if (this.currBranch == null) {
                    this.currLineIndex = -1;
                }
                else {
                    if (value >= this.currBranch.actorLines.Count) {
                        this.FinishEntry();
                    }
                    else {
                        this.currLineIndex = value;
                        if (this.actors.TryGetValue(this.currBranch.actorLines[this.currLineIndex].actorId, out var actor))
                            actor.Line = this.currBranch.actorLines[this.currLineIndex].line;
                    }
                }
            }
        }
        #endregion

        #region Methods
        public bool TryEnter(string entryName)
        {
            if (!this.dialogScript.ContainsKey(entryName))
            {
                Debug.LogWarning($"No entry named {entryName}.");
                return false;
            }

            var entry = this.dialogScript[entryName];
            foreach (var branch in entry)
            {
                bool found = true;
                foreach (var condition in branch.conditions)
                {
                    if (this.DialogState.TryGetValue(condition.Key, out var value) && value != condition.Value)
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    this.currBranch = branch;
                    this.CurrLineIndex = 0;
                    return true;
                }
            }

            return false;
        }
        public void NextLine(string callFromActor)
        {
            // If not from the active actor, then ignore it.
            if (this.currBranch != null && this.currBranch.actorLines[this.CurrLineIndex].actorId == callFromActor)
                this.CurrLineIndex++;
        }
        private void FinishEntry()
        {
            foreach (var result in this.currBranch.results)
                this.DialogState[result.Key] = result.Value;
            if (this.currBranch.nextEntry != "")
                this.TryEnter(this.currBranch.nextEntry);
        }


        public void AddActor(TextActor actor) => this.actors.Add(actor.actorId, actor);
        public void RemoveActor(TextActor actor) => this.actors.Remove(actor.actorId);


        public void Invoke()
        {
            var extensionsIni = new[] { new SFB.ExtensionFilter("Ini Files", "ini") };
            void callbackIni(string[] paths)
            {
                if (paths.Length > 0)
                    this.ReadDialogIni(paths[0]);
            }
            SFB.StandaloneFileBrowser.OpenFilePanelAsync("Load Ini", "", extensionsIni, false, callbackIni);

            var extensionsTxt = new[] { new SFB.ExtensionFilter("Text Files", "txt") };
            void callbackTxt(string[] paths)
            {
                if (paths.Length > 0)
                {
                    this.ReadDialogScript(paths[0]);
                    this.TryEnter("amysay");
                }
            }
            SFB.StandaloneFileBrowser.OpenFilePanelAsync("Load Dialogs", "", extensionsTxt, false, callbackTxt);
        }
        #endregion
    }

    public class EntryBranch
    {
        public List<ActorLine> actorLines;
        public List<KeyValuePair<string, string>> conditions;
        public List<KeyValuePair<string, string>> results;
        public string nextEntry;

        public EntryBranch()
        {
            this.actorLines = new();
            this.conditions = new();
            this.results = new();
            this.nextEntry = "";
        }

        public struct ActorLine
        {
            public string actorId;
            public string line;
        }
    }
}
