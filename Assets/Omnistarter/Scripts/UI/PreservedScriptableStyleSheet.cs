// author: Omnistudio
// version: 2025.05.04

using Omnis.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.UI
{
    [CreateAssetMenu(menuName = "Omnis/Preserved Style Sheet", order = 243)]
    public sealed class PreservedScriptableStyleSheet : ScriptableStyleSheet
    {
        private readonly List<RichTextScriptableTag> tags = new()
        {
            #region Offset
            new() {
                name = "elastic",
                Tune = (tmpro, info, time) =>
                    tmpro.MoveChar(
                        (index) => 10f * Easing.InBounce((time - index * 0.133f).PingPong(1f)).ono(),
                        info.startIndex, info.endIndex
                    )
            },
            new() {
                name = "float",
                Tune = (tmpro, info, time) =>
                    tmpro.MoveChar(
                        (index) => 10f * Easing.RawSine((time - Spectrum(info, index)).Repeat(1f)).ono(),
                        info.startIndex, info.endIndex
                    )
            },
            #endregion

            #region Emphasis
            new() {
                name = "highlight",
                Tune = (tmpro, info, time) =>
                    tmpro.PaintChar(
                        (index) => ColorHelper.Lerp(ColorHelper.skyBlue, ColorHelper.gold, Spectrum(info, index)),
                        info.startIndex, info.endIndex
                    )
            },
            new() {
                name = "rgb",
                Tune = (tmpro, info, time) =>
                    tmpro.PaintChar(
                        (index) => Color.HSVToRGB((time - Spectrum(info, index)).Repeat(1f), 1f, 1f),
                        info.startIndex, info.endIndex
                    )
            },
            #endregion

            #region Reveal
            new() {
                name = "reveallinear", delta = -0.05f,
                //Tune = (tmpro, info, time) =>
                //    (255f * time.Clamp01()).RoundToInt()
            },
            new() {
                name = "revealstep", delta = -0.05f,
                //tuneFunc = (raw, time) => raw.Replace("?", (255f * time.Step01(0.1f)).RoundToInt().ToString("X2"))
            },
            #endregion
        };
        public override List<RichTextScriptableTag> Tags => tags;

        private static float Spectrum(TagInfo tagInfo, int index)
            => tagInfo.endIndex == tagInfo.startIndex ? 0f : index / (float)(tagInfo.endIndex - tagInfo.startIndex);
    }
}
