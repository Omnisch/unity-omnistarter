// author: Omnistudio
// version: 2025.06.11

using System.Collections.Generic;
using UnityEngine;

namespace Omnis.UI
{
    public partial class TextManager : MonoBehaviour
    {
        #region Serialized Fields
        public List<TextActor> actors = new();
        [SerializeField] private ScriptableStyleSheet styleSheet;

        [HideInInspector] public static readonly float DefaultPrintSpeed = 20f;
        #endregion

        #region Properties
        public ScriptableStyleSheet StyleSheet => styleSheet;
        #endregion

        #region Methods
        public void Invoke()
        {
            actors.Find(actor => actor.actorId == "Test").Line =
                "<print>望长城内外，惟余莽莽；</print><break wait=1><print>大河上下，顿失滔滔。</print>";
        }
        #endregion

        #region Unity Methods
        #endregion
    }
}
