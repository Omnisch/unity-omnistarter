// author: Omnistudio
// version: 2025.03.31

using Omnis.Utils;
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

        #region Fields
        public ScriptableStyleSheet StyleSheet => styleSheet;
        #endregion

        #region Methods
        public void Invoke()
        {
            actors.Find(actor => actor.actorName == "Test").Line =
                "<line-height=1.2em><elastic>俱往矣，数<gold>风流人物</gold>，</elastic>还看今朝。\n" +
                "The quick brown fox <reveal>jumps over the lazy dog.</reveal>";
        }
        #endregion

        #region Unity Methods
        #endregion
    }
}
