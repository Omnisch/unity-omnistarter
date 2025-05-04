// author: Omnistudio
// version: 2025.05.04

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
            actors.Find(actor => actor.actorId == "Test").Line =
                "<float>俱往矣，数<highlight>风流人物</highlight>，还看今朝。</float>\n" +
                "The quick brown fox jumps over the lazy dog.";
        }
        #endregion

        #region Unity Methods
        #endregion
    }
}
