// author: Omnistudio
// version: 2025.07.06

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
        private Dictionary<string, string> dialogState;
        private Dictionary<string, List<EntryBranch>> dialogScript;
        private EntryBranch currBranch;
        #endregion

        #region Properties
        public ScriptableStyleSheet StyleSheet => styleSheet;
        private int currLineIndex;
        protected int CurrLineIndex
        {
            get => currLineIndex;
            set
            {
                Debug.Log(value);
                if (currBranch == null)
                {
                    currLineIndex = -1;
                }
                else
                {
                    if (value >= currBranch.actorLines.Count)
                    {
                        FinishEntry();
                    }
                    else
                    {
                        currLineIndex = value;
                        if (actors.TryGetValue(currBranch.actorLines[currLineIndex].actorId, out var actor))
                            actor.Line = currBranch.actorLines[currLineIndex].line;
                    }
                }
            }
        }
        #endregion

        #region Methods
        public bool TryEnter(string entryName)
        {
            if (!dialogScript.ContainsKey(entryName))
            {
                Debug.LogWarning($"No entry named {entryName}.");
                return false;
            }

            var entry = dialogScript[entryName];
            foreach (var branch in entry)
            {
                bool found = true;
                foreach (var condition in branch.conditions)
                {
                    if (dialogState.TryGetValue(condition.Key, out var value) && value != condition.Value)
                    {
                        found = false;
                        break;
                    }
                }

                if (found)
                {
                    currBranch = branch;
                    NextLine(true);
                    return true;
                }
            }

            return false;
        }
        public void NextLine(bool fromStart = false)
        {
            if (fromStart)
                CurrLineIndex = 0;
            else
                CurrLineIndex++;
        }
        public void FinishEntry()
        {
            foreach (var result in currBranch.results)
                dialogState[result.Key] = result.Value;
            if (currBranch.nextEntry != "")
                TryEnter(currBranch.nextEntry);
        }


        public void AddActor(TextActor actor) => actors.Add(actor.actorId, actor);
        public void RemoveActor(TextActor actor) => actors.Remove(actor.actorId);


        public void Invoke()
        {
            IO.OpenBrowserAndDo("*.ini | *.ini", path => ReadDialogIni(path));
            IO.OpenBrowserAndDo("*.txt | *.txt", path => {
                ReadDialogScript(path);
                TryEnter("amysay");
            });
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
