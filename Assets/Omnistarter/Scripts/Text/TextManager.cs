// author: Omnistudio
// version: 2025.07.06

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

        #region Properties
        public ScriptableStyleSheet StyleSheet => styleSheet;
        #endregion

        #region Methods
        public void Invoke()
        {
            actors.Find(actor => actor.actorId == "Test").Line =
                "<mousepush><print>望长城内外，惟余莽莽；<break time=.5>大河上下，顿失滔滔。";
        }
        #endregion
    }
}
