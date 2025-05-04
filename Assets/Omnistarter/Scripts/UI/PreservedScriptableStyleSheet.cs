// author: Omnistudio
// version: 2025.05.05

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
            new("elastic",
                (c) => SimpleEditVertices(c, 10f * Easing.InBounce((c.time - c.index * 0.133f).PingPong(1f)).ono())),
            new("float",
                (c) => SimpleEditVertices(c, 10f * Easing.RawSine((c.time - Spectrum(c)).Repeat(1f)).ono())),
            #endregion

            #region Emphasis
            new("highlight",
                (c) => SimpleEditColor(c, ColorHelper.Lerp(ColorHelper.skyBlue, ColorHelper.gold, Spectrum(c)))),
            new("rgb",
                (c) => SimpleEditColor(c, Color.HSVToRGB((c.time - Spectrum(c)).Repeat(1f), 1f, 1f))),
            #endregion

            #region Reveal
            new("reveallinear",
                (c) => {
                    if (c.tmpro.textInfo.characterInfo[c.index].character == '¾ã') Debug.Log((byte)(255 * (c.time - c.index * 0.05f).Clamp01()));
                    SimpleEditAlpha(c, (byte)(255 * (c.time - c.index * 0.05f).Clamp01()));
                }),
            new("revealstep",
                (c) => SimpleEditAlpha(c, (byte)(255 * (c.time - c.index * 0.05f).Step01(0.1f)))),
            #endregion
        };
        public override List<RichTextScriptableTag> Tags => tags;

        #region Methods
        private static float Spectrum(CharInfo c)
            => c.tagInfo.endIndex <= c.tagInfo.startIndex + 1 ? 0f : (c.index - c.tagInfo.startIndex) / (float)(c.tagInfo.endIndex - c.tagInfo.startIndex - 1);
        private static void SimpleEditVertices(CharInfo c, Vector3 value)
        {
            var charInfo = c.tmpro.textInfo.characterInfo[c.index];
            if (!charInfo.isVisible) return;
            int matIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Vector3[] vertices = c.tmpro.textInfo.meshInfo[matIndex].vertices;
            for (int i = 0; i < 4; i++)
                vertices[vertexIndex + i] += value;
        }
        private static void SimpleEditColor(CharInfo c, Color32 value)
        {
            var charInfo = c.tmpro.textInfo.characterInfo[c.index];
            if (!charInfo.isVisible) return;
            int matIndex = charInfo.materialReferenceIndex;
            Color32[] colors32 = c.tmpro.textInfo.meshInfo[matIndex].colors32;
            int vertexIndex = charInfo.vertexIndex;
            for (int i = 0; i < 4; i++)
                colors32[vertexIndex + i] = value;
        }
        private static void SimpleEditAlpha(CharInfo c, byte value)
        {
            var charInfo = c.tmpro.textInfo.characterInfo[c.index];
            if (!charInfo.isVisible) return;
            int matIndex = charInfo.materialReferenceIndex;
            Color32[] colors32 = c.tmpro.textInfo.meshInfo[matIndex].colors32;
            int vertexIndex = charInfo.vertexIndex;
            for (int i = 0; i < 4; i++)
                colors32[vertexIndex + i].a = value;
        }
        #endregion
    }
}
