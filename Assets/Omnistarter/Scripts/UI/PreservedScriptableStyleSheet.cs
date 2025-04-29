// author: Omnistudio
// version: 2025.04.01

using Omnis.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.UI
{
    [CreateAssetMenu(menuName = "Omnis/Preserved Style Sheet", order = 243)]
    public class PreservedScriptableStyleSheet : ScriptableStyleSheet
    {
        private readonly List<RichTextScriptableTag> tags = new()
        {
            new() {
                name = "elastic",
                OpeningTag = "<voffset=?em>",
                ClosingTag = "</voffset>",
                paramType = ScriptableTagParamType.TimeWithDelta,
                delta = -0.133f,
                tuneFunc = (raw, param) => raw.Replace("?", (0.4f * Easing.InBounce(param.PingPong(1f))).ToString("F2"))
            },
            new() {
                name = "float",
                OpeningTag = "<voffset=?em>",
                ClosingTag = "</voffset>",
                paramType = ScriptableTagParamType.TimeWithSpectrum,
                spectrumLength = -1f,
                tuneFunc = (raw, param) => raw.Replace("?", (0.1f * Easing.RawSine(param.Repeat(1f))).ToString("F2"))
            },
            new() {
                name = "colorful",
                OpeningTag = "<color=#?>",
                ClosingTag = "</color>",
                paramType = ScriptableTagParamType.Spectrum,
                spectrumLength = 1f,
                tuneFunc = (raw, param) => raw.Replace("?", ColorUtility.ToHtmlStringRGB(
                    ColorHelper.LerpFromColorToColor(ColorHelper.skyBlue, ColorHelper.gold, param))
                )
            },
            new() {
                name = "reveallinear",
                OpeningTag = "<alpha=#?>",
                ClosingTag = "<alpha=#FF>",
                paramType = ScriptableTagParamType.TimeWithDelta,
                delta = -0.1f,
                tuneFunc = (raw, param) => raw.Replace("?", (255f * param.Clamp01()).RoundToInt().ToString("X2"))
            },
            new() {
                name = "revealstep",
                OpeningTag = "<alpha=#?>",
                ClosingTag = "<alpha=#FF>",
                paramType = ScriptableTagParamType.TimeWithDelta,
                delta = -0.05f,
                tuneFunc = (raw, param) => raw.Replace("?", (255f * param.Step01(0.5f)).RoundToInt().ToString("X2"))
            },
        };
        public override List<RichTextScriptableTag> Tags => tags;
    }
}
