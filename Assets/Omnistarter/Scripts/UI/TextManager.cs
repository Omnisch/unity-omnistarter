// author: Omnistudio
// version: 2025.06.17

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
                "<mousepush><reveal>望长城内外，惟余<float>莽莽</float>；</reveal><break>\n<reveal>大河上下，顿失滔滔。</reveal>";
        }
        #endregion
    }
}
