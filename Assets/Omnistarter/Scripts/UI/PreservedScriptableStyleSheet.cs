// author: Omnistudio
// version: 2025.06.11

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
                tune: (c) => c.SimpleEditVertices(10f * Easing.InBounce((c.time - c.index * 0.133f).PingPong(1f)).ono())),
            new(name: "float",
                tune: (c) => c.SimpleEditVertices(10f * Easing.RawSine((c.time - c.Spectrum()).Repeat(1f)).ono())),
            new(name: "pacing",
                tune: (c) => {
                    float x = 5f * Easing.RawSine((c.time - c.Spectrum() - 0.25f).Repeat(1f));
                    float y = 5f * Easing.RawSine((c.time - c.Spectrum()).Repeat(1f));
                    c.SimpleEditVertices(new Vector3(x, y, 0f));
                }),
            #endregion

            #region Emphasis
            new(name: "hili",
                tune: (c) => c.SimpleEditColor(ColorHelper.Lerp(ColorHelper.skyBlue, ColorHelper.gold, c.Spectrum()))),
            new(name: "rgb",
                tune: (c) => c.SimpleEditColor(Color.HSVToRGB((c.time - c.Spectrum()).Repeat(1f), 1f, 1f))),
            #endregion

            #region Print
            new(name: "break",
                tune: (c) => {
                    void Escape()
                    {
                        c.actor.PrintSpeed = TextManager.DefaultPrintSpeed;
                        c.tagInfo.finished = true;
                    }
                    if ((int)c.actor.PrintPast == c.tagInfo.startIndex)
                    {
                        if (c.tagInfo.attrs.TryGetValue("wait", out string o) && float.TryParse(o, out float wait))
                            c.actor.PrintSpeed = 1f / wait;
                        else
                            c.actor.PrintSpeed = 0f;
                        if (c.actor.LeftPressed) Escape();
                    }
                    else if ((int)c.actor.PrintPast > c.tagInfo.startIndex) Escape();
                }),
            new(name: "reveal",
                tune: (c) => {
                    // TODO
                    c.SimpleEditAlpha((c.actor.PrintPast - c.index).Clamp01());
                }),
            new(name: "print",
                tune: (c) => {
                    // All shown, stop calculations.
                    if ((int)c.actor.PrintPast >= c.tagInfo.endIndex)
                        c.tagInfo.finished = true;

                    // Start printing.
                    else if ((int)c.actor.PrintPast == c.tagInfo.startIndex)
                    {
                        if (c.tagInfo.attrs.TryGetValue("speed", out string o) && float.TryParse(o, out float speed))
                            c.actor.PrintSpeed = speed;
                    }

                    // Click to skip printing.
                    else if ((int)c.actor.PrintPast > c.tagInfo.startIndex)
                        if (c.actor.LeftPressed)
                            c.actor.PrintPast = c.tagInfo.endIndex;

                    c.SimpleEditAlpha(Mathf.Clamp01((int)c.actor.PrintPast - c.index));
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
