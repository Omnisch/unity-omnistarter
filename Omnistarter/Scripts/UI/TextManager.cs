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
        #endregion

        #region Fields
        #endregion

        #region Methods
        public void Invoke()
        {
            RichTextScriptableTag floatTag = new()
            {
                id = "float",
                applyToCharWithPhase = false,
                OpeningTag = "<voffset=?em>",
                ClosingTag = "</voffset>",
                tuneFunc = (raw, phase) => raw.Replace("?", (0.2f * Easing.RawSine((Time.timeSinceLevelLoad + phase).Repeat(1f))).ToString("F2"))
            };
            RichTextScriptableTag elasticTag = new()
            {
                id = "elastic",
                applyToCharWithPhase = true,
                OpeningTag = "<voffset=?em>",
                ClosingTag = "</voffset>",
                tuneFunc = (raw, phase) => raw.Replace("?", (0.4f * Easing.InBounce((Time.timeSinceLevelLoad + phase).PingPong(1f))).ToString("F2"))
            };
            RichTextScriptableTag goldTag = new()
            {
                id = "gold",
                applyToCharWithPhase = false,
                OpeningTag = "<color=#?>",
                ClosingTag = "</color>",
                tuneFunc = (raw, phase) => raw.Replace("?", ColorUtility.ToHtmlStringRGB(
                    ColorHelper.LerpFromColorToColor(ColorHelper.appleBlack, ColorHelper.gold, Easing.InOutSine(Time.timeSinceLevelLoad + phase).PingPong(1f)))
                )
            };

            actors.Find(actor => actor.actorName == "Test").Line = new RichTextLine()
            {
                text = "<line-height=1.2em><elastic>俱往矣，数<gold>风流人物</gold>，</elastic>还看今朝。\n" +
                       "The quick brown fox <elastic>jumps over the lazy dog.</elastic>",
                tags = new() { elasticTag, floatTag, goldTag }
            };
        }
        #endregion

        #region Unity Methods
        #endregion
    }
}
