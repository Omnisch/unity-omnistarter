// author: Omnistudio
// version: 2025.03.31

using Omnis.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.UI
{
    [CreateAssetMenu(menuName = "Omnis/Preserved Style Sheet", order = 243)]
    public class PreservedScriptableStyleSheet : ScriptableStyleSheet
    {
        [Omnis.Editor.InspectorReadOnly]
        [SerializeField] private List<RichTextScriptableTag> tags = new()
        {
            new() {
                id = "elastic",
                applyToCharWithPhase = true,
                OpeningTag = "<voffset=?em>",
                ClosingTag = "</voffset>",
                tuneFunc = (raw, phase) => raw.Replace("?", (0.4f * Easing.InBounce((Time.timeSinceLevelLoad + phase).PingPong(1f))).ToString("F2"))
            },
            new() {
                id = "float",
                applyToCharWithPhase = false,
                OpeningTag = "<voffset=?em>",
                ClosingTag = "</voffset>",
                tuneFunc = (raw, phase) => raw.Replace("?", (0.2f * Easing.RawSine((Time.timeSinceLevelLoad + phase).Repeat(1f))).ToString("F2"))
            },
            new() {
                id = "gold",
                applyToCharWithPhase = false,
                OpeningTag = "<color=#?>",
                ClosingTag = "</color>",
                tuneFunc = (raw, phase) => raw.Replace("?", ColorUtility.ToHtmlStringRGB(
                    ColorHelper.LerpFromColorToColor(ColorHelper.appleBlack, ColorHelper.gold, Easing.InOutSine(Time.timeSinceLevelLoad + phase).PingPong(1f)))
                )
            },
            new() {
                id = "reveal",
                applyToCharWithPhase = true,
                OpeningTag = "<alpha=#?>",
                ClosingTag = "<alpha=#00>",
                tuneFunc = (raw, phase) => raw.Replace("?", (255f * Easing.Smoothstep((Time.timeSinceLevelLoad + phase).PingPong(1f))).RoundToInt().ToString("X2"))
            },
        };
        public override List<RichTextScriptableTag> Tags => tags;
    }
}
