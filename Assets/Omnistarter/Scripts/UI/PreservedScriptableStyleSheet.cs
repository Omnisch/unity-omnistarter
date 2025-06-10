// author: Omnistudio
// version: 2025.06.10

using Omnis.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.UI
{
    [CreateAssetMenu(menuName = "Omnis/Preserved Style Sheet", order = 243)]
    public sealed class PreservedScriptableStyleSheet : ScriptableStyleSheet
    {
        public override List<ScriptableRichTextTag> Tags => tags;
        private readonly List<ScriptableRichTextTag> tags = new()
        {
            #region Offset
            new(name: "elastic",
                tune: (c) => c.SimpleEditVertices(10f * Easing.InBounce((c.actor.LineTimePast - c.index * 0.133f).PingPong(1f)).ono())),
            new(name: "float",
                tune: (c) => c.SimpleEditVertices(10f * Easing.RawSine((c.actor.LineTimePast - c.Spectrum()).Repeat(1f)).ono())),
            new(name: "pacing",
                tune: (c) => {
                    float x = 5f * Easing.RawSine((c.actor.LineTimePast - c.Spectrum() - 0.25f).Repeat(1f));
                    float y = 5f * Easing.RawSine((c.actor.LineTimePast - c.Spectrum()).Repeat(1f));
                    c.SimpleEditVertices(new Vector3(x, y, 0f));
                }),
            #endregion

            #region Emphasis
            new(name: "hili",
                tune: (c) => c.SimpleEditColor(ColorHelper.Lerp(ColorHelper.skyBlue, ColorHelper.gold, c.Spectrum()))),
            new(name: "rgb",
                tune: (c) => c.SimpleEditColor(Color.HSVToRGB((c.actor.LineTimePast - c.Spectrum()).Repeat(1f), 1f, 1f))),
            #endregion

            #region Reveal
            new(name: "showlnr",
                tune: (c) => {
                    c.SimpleEditAlpha((byte)(255 * (c.actor.LineTimePast - c.index * 0.05f).Clamp01()));
                }),
            new(name: "showstp",
                tune: (c) => {
                    if (c.actor.LineTimePast > (c.tagInfo.endIndex + 1) * 0.05f)
                            c.tagInfo.finished = true;
                    else if (c.actor.LineTimePast >= c.tagInfo.startIndex * 0.05f && c.tagInfo.startIndex == c.index)
                        if (c.tagInfo.attrs.TryGetValue("speed", out string a) && float.TryParse(a, out float speed))
                            c.actor.LineTimeScale = speed;
                    c.SimpleEditAlpha((byte)(255 * (c.actor.LineTimePast - c.index * 0.05f).Step01(0.1f)));
                }),
            #endregion

            #region Interact
            new(name: "curpush",
                tune: (c) => {
                    var charInfo = c.actor.TMPro.textInfo.characterInfo[c.index];
                    if (!charInfo.isVisible) return;
                    int matIndex = charInfo.materialReferenceIndex;
                    int vertexIndex = charInfo.vertexIndex;
                    Vector3[] vertices = c.actor.TMPro.textInfo.meshInfo[matIndex].vertices;

                    Vector3 charCenter = new();
                    for (int i = 0; i < 4; i++)
                        charCenter += vertices[vertexIndex + i];
                    charCenter /= 4f;

                    Vector3 v =  (c.actor.TMPro.transform.position + charCenter) - c.mousePosition;
                    float dst = v.magnitude;
                    if (dst > 100f) return;
                    v = v.normalized;
                    for (int i = 0; i < 4; i++)
                        vertices[vertexIndex + i] += 2f * (10f - dst.Sqrt()) * v;
                })
            #endregion
        };
    }
}
