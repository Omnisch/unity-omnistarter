// author: Omnistudio
// version: 2025.07.06

using System.Collections.Generic;
using UnityEngine;

namespace Omnis.Text
{
    public partial class TextActor
    {
        #region Fields
        private Dictionary<string, List<EntryBranch>> script;
        private EntryBranch currBranch;
        #endregion

        #region Methods
        public bool TrySpeak(string entryName)
        {
            if (!script.ContainsKey(entryName))
            {
                Debug.LogWarning($"No entry named {entryName}.");
                return false;
            }

            var entry = script[entryName];
            foreach (var branch in entry)
            {
                bool found = false;
                foreach (var condition in branch.conditions)
                {
                    if (TextManager.Instance.dialogStateDict.TryGetValue(condition.Key, out var value) && value == condition.Value)
                    {
                        //Debug.Log($"Condition {condition.Key} not met.");
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    currBranch = branch;
                    Line = branch.text;
                    return true;
                }
            }

            return false;
        }
        public void FinishEntry()
        {
            StopAllCoroutines();
            foreach (var result in currBranch.results)
                TextManager.Instance.dialogStateDict[result.Key] = result.Value;
            TextManager.Instance.CallEntry(currBranch.nextEntry);
        }
        #endregion

        private class EntryBranch
        {
            public string text;
            public List<KeyValuePair<string, string>> conditions;
            public List<KeyValuePair<string, string>> results;
            public string nextEntry;
        }
    }
}
