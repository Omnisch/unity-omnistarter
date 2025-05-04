// author: Omnistudio
// version: 2025.05.05

using System.Collections.Generic;
using UnityEngine;

namespace Omnis.UI
{
    public partial class TextManager : MonoBehaviour
    {
        #region Serialized Fields
        public List<TextActor> actors = new();
        [SerializeField] private ScriptableStyleSheet styleSheet;
        #endregion

        #region Properties
        public ScriptableStyleSheet StyleSheet => styleSheet;
        #endregion

        #region Methods
        #endregion

        #region Unity Methods
        #endregion
    }
}
