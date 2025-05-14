// author: Omnistudio
// version: 2025.05.14

using Omnis.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.UI
{
    [CreateAssetMenu(menuName = "Omnis/Preserved Style Sheet", order = 243)]
    public sealed class PreservedScriptableStyleSheet : ScriptableStyleSheet
    {
        private readonly List<ScriptableRichTextTag> tags = new()
        {
            #region Offset
            new("elastic",
                (c) => SimpleEditVertices(c, 10f * Easing.InBounce((c.time - c.index * 0.133f).PingPong(1f)).ono())),
            new("float",
                (c) => SimpleEditVertices(c, 10f * Easing.RawSine((c.time - Spectrum(c)).Repeat(1f)).ono())),
            new("pacing",
                (c) => {
                    float x = 5f * Easing.RawSine((c.time - Spectrum(c) - 0.25f).Repeat(1f));
                    float y = 5f * Easing.RawSine((c.time - Spectrum(c)).Repeat(1f));
                    SimpleEditVertices(c, new Vector3(x, y, 0f));
                }),
            #endregion

            #region Emphasis
            new("hili",
                (c) => SimpleEditColor(c, ColorHelper.Lerp(ColorHelper.skyBlue, ColorHelper.gold, Spectrum(c)))),
            new("rgb",
                (c) => SimpleEditColor(c, Color.HSVToRGB((c.time - Spectrum(c)).Repeat(1f), 1f, 1f))),
            #endregion

            #region Reveal
            new("showlnr",
                (c) => {
                    if (c.tagInfo.attrs.TryGetValue("speed", out string a) && float.TryParse(a, out float speed))
                        SimpleEditAlpha(c, (byte)(255 * (c.time - c.index * 0.05f / speed).Clamp01()));
                    else
                        SimpleEditAlpha(c, (byte)(255 * (c.time - c.index * 0.05f).Clamp01()));
                }),
            new("showstp",
                (c) => {
                    if (c.tagInfo.attrs.TryGetValue("speed", out string a) && float.TryParse(a, out float speed))
                        SimpleEditAlpha(c, (byte)(255 * (c.time - c.index * 0.05f / speed).Step01(0.1f)));
                    else
                        SimpleEditAlpha(c, (byte)(255 * (c.time - c.index * 0.05f).Clamp01()));
                }),
            #endregion

            #region Interact
            new("curpush",
                (c) => {
                    var charInfo = c.tmpro.textInfo.characterInfo[c.index];
                    if (!charInfo.isVisible) return;
                    int matIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;
                    Vector3[] vertices = c.tmpro.textInfo.meshInfo[matIndex].vertices;

                    Vector3 charCenter = new();
                    for (int i = 0; i < 4; i++)
                        charCenter += vertices[vertexIndex + i];
                    charCenter /= 4f;

                    Vector3 v =  (c.tmpro.transform.position + charCenter) - c.mousePosition;
                    float dst = v.magnitude;
                    if (dst > 100f) return;
                    v = v.normalized;
                    for (int i = 0; i < 4; i++)
                        vertices[vertexIndex + i] += 2f * (10f - dst.Sqrt()) * v;
                })
            #endregion
        };
        public override List<ScriptableRichTextTag> Tags => tags;

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
