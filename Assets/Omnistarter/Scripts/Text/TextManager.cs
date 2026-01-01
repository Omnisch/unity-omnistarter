// author: Omnistudio
// version: 2025.11.28

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
                        if (actors.TryGetValue(currBranch.actorLines[currLineIndex].actorId, out var actor))
                            actor.Line = currBranch.actorLines[currLineIndex].line;
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
                bool found = true;
                foreach (var condition in branch.conditions) {
                    if (DialogState.TryGetValue(condition.Key, out var value) && value != condition.Value) {
                        found = false;
                        break;
                    }
                }

                if (found) {
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
            foreach (var result in currBranch.results)
                DialogState[result.Key] = result.Value;
            if (currBranch.nextEntry != "")
                TryEnter(currBranch.nextEntry);
        }


        public void AddActor(TextActor actor) => actors.Add(actor.actorId, actor);
        public void RemoveActor(TextActor actor) => actors.Remove(actor.actorId);


        public void Invoke() {
            var extensionsIni = new[] { new SFB.ExtensionFilter("Ini Files", "ini") };
            void callbackIni(string[] paths) {
                if (paths.Length > 0)
                    ReadDialogIni(paths[0]);
            }
            SFB.StandaloneFileBrowser.OpenFilePanelAsync("Load Ini", "", extensionsIni, false, callbackIni);

            var extensionsTxt = new[] { new SFB.ExtensionFilter("Text Files", "txt") };
            void callbackTxt(string[] paths) {
                if (paths.Length > 0) {
                    ReadDialogScript(paths[0]);
                    TryEnter("amysay");
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

        public EntryBranch() {
            actorLines = new();
            conditions = new();
            results = new();
            nextEntry = "";
        }

        public struct ActorLine
        {
            public string actorId;
            public string line;
        }
    }
}
