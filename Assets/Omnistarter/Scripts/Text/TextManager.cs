// author: Omnistudio
// version: 2025.07.06

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.Text
{
    public partial class TextManager : MonoBehaviour
    {
        #region Serialized Fields
        public List<TextActor> actors = new();
        [SerializeField] private ScriptableStyleSheet styleSheet;
        #endregion

        #region Fields
        public Dictionary<string, string> dialogStateDict;
        public Dictionary<string, Action> dialogEntries;
        #endregion

        #region Properties
        public ScriptableStyleSheet StyleSheet => styleSheet;
        #endregion

        #region Methods
        public void CallEntry(string entryName)
        {
            if (dialogEntries.TryGetValue(entryName, out var entryAction))
                entryAction.Invoke();
        }
        public void Invoke()
        {
            IO.OpenBrowserAndDo("*.ini | *.ini", path => ReadDialogIni(path));
            IO.OpenBrowserAndDo("*.txt | *.txt", path => ReadDialog(path));
        }
        #endregion

        #region Unity Methods
        private void Start()
        {
            dialogStateDict = new();
        }
        #endregion
    }
}
