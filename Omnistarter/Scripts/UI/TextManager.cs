// author: Omnistudio
// version: 2025.04.01

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

        #region Properties
        public ScriptableStyleSheet StyleSheet => styleSheet;
        #endregion

        #region Methods
        public void Invoke()
        {
            actors.Find(actor => actor.actorName == "Test").Line =
                "<revealstep><elastic>俱往矣，数<colorful>风流人物</colorful>，</elastic>还看今朝。\n" +
                "The quick brown fox jumps over the lazy dog.</revealstep>";
        }
        #endregion

        #region Unity Methods
        #endregion
    }
}
