// author: Omnistudio
// version: 2025.07.06

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Omnis.Text
{
    public partial class TextManager
    {
        #region Serialized Fields
        #endregion

        #region Fields
        #endregion

        #region Properties
        #endregion

        #region Methods
        public void ReadDialogIni(string path)
        {
            IniStorage ini = new(path);
            foreach (var line in ini.pairs)
            {
                dialogStateDict.TryAdd(line.Name, line.Get());
                Debug.Log($"[Dialog Ini] {line.Name}: {line.Get()}");
            }
        }
        public void ReadDialog(string path)
        {
            string[] texts = File.ReadAllLines(path);
            foreach (string text in texts)
            {
                var trimmed = text.Trim();
                if (trimmed.Length == 0)
                    continue;
                switch (trimmed[0])
                {
                    // EntryBranch (Trigger)
                    case '[':
                        break;
                    // Initial line
                    case '.':
                        break;
                    // Successive line
                    case ',':
                        break;
                    // Condition
                    case '>':
                        break;
                    // Result
                    case '<':
                        break;
                }
            }
        }
        #endregion
    }
}
