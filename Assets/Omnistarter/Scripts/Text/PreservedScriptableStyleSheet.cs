// author: Omnistudio
// version: 2026.03.18

using Omnis.Utils;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.Text
{
    [CreateAssetMenu(menuName = "Omnis/Presets/Preserved Style Sheet", order = 245)]
    public class PreservedScriptableStyleSheet : ScriptableStyleSheet
    {
        public override List<ScriptableRichTextTag> Tags => this.tags;
        protected readonly List<ScriptableRichTextTag> tags = new()
        {
            #region Offset
            // <elastic> </elastic>
            new(name: "elastic",
                render: c => c.SimpleEditVertices(c.actor.AnimFactor * Easing.BounceIn((c.time - c.index * 0.133f).PingPong(1f)).ono())),
            // <float> </float>
            new(name: "float",
                render: c => c.SimpleEditVertices(c.actor.AnimFactor * Easing.SineRaw((c.time - c.Spectrum()).Repeat(1f)).ono())),
            // <pacing> </pacing>
            new(name: "pacing",
                render: c => {
                    float x = c.actor.AnimFactor * Easing.SineRaw((c.time - c.Spectrum() - 0.25f).Repeat(1f));
                    float y = c.actor.AnimFactor * Easing.SineRaw((c.time - c.Spectrum()).Repeat(1f));
                    c.SimpleEditVertices(new Vector3(x, y, 0f));
                }),
            // <noise (speed=float)> </noise>
            // speed=4 can be panic enough
            new (name: "noise",
                render: c => {
                    const float a = 1f;
                    float f = c.time;
                    
                    if (c.tagInfo.attrs.TryGetValue("speed", out string o) && float.TryParse(o, out float speed)) {
                        f *= speed;
                        f *= speed;
                    }
                    float x = c.actor.AnimFactor * Mathf.PerlinNoise(f, c.index);
                    float y = c.actor.AnimFactor * Mathf.PerlinNoise(f + a, c.index);
                    
                    c.SimpleEditVertices(new Vector3(x, y, 0f));
                }),
            #endregion

            #region Emphasis
            // <hili> </hili>
            new(name: "hili",
                render: c => c.SimpleEditColor(ColorHelper.gold)),
            // <colorful> </colorful>
            new(name: "colorful",
                render: c => c.SimpleEditColor(ColorHelper.Lerp(ColorHelper.skyBlue, ColorHelper.gold, c.Spectrum()))),
            // <rgb> </rgb>
            new(name: "rgb",
                render: c => c.SimpleEditColor(Color.HSVToRGB((c.time - c.Spectrum()).Repeat(1f), 1f, 1f))),
            #endregion

            #region Print
            // <break (time=float) />
            new(name: "break",
                render: c => {
                    if (c.actor.pi.past >= c.tagInfo.endIndex)
                    {
                        c.tagInfo.finished = true;
                    }
                    else if (c.actor.pi.past >= c.tagInfo.startIndex)
                    {
                        // Click to skip the break.
                        if (c.actor.Next)
                        {
                            c.actor.Next = false;
                            c.actor.pi.pause = false;
                            c.tagInfo.finished = true;
                            return;
                        }
                        if (!c.tagInfo.active)
                        {
                            c.tagInfo.active = true;
                            if (c.tagInfo.attrs.TryGetValue("time", out string o) && float.TryParse(o, out float wait))
                            {
                                c.actor.pi.pause = true;
                                c.actor.DoAfterSeconds(wait, () => c.actor.pi.pause = false);
                            }
                            else
                                c.actor.pi.pause = true;
                        }
                    }
                }),
            // <reveal (speed=float) (no-skip)> </reveal>
            new(name: "reveal",
                render: c => {
                    PrintingPresets(c);
                    c.SimpleEditAlpha(Mathf.Clamp01(c.actor.pi.past - c.index));
                }),
            // <print (speed=float) (no-skip)> </print>
            new(name: "print",
                render: c => {
                    PrintingPresets(c);
                    c.SimpleEditAlpha(Mathf.Clamp01((int)c.actor.pi.past - c.index));
                }),
            #endregion

            #region Interact
            // <pushing> </pushing>
            new(name: "pushing",
                render: c => {
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
                        vertices[vertexIndex + i] += 0.1f * c.actor.AnimFactor * (10f - dst.Sqrt()) * v;
                })
            #endregion
        };

        private static void PrintingPresets(CharInfo c)
        {
            // All shown, stop calculations.
            if (c.actor.pi.past >= c.tagInfo.endIndex) {
                c.tagInfo.finished = true;
                return;
            } else {
                c.tagInfo.finished = false;
            }

            // Start printing.
            if (c.actor.pi.past >= c.tagInfo.startIndex && !c.tagInfo.active) {
                c.tagInfo.active = true;
                if (c.tagInfo.attrs.TryGetValue("speed", out string o) && float.TryParse(o, out float speed))
                    c.actor.pi.speed = speed;
            }

            // Click to skip printing.
            else if (c.actor.pi.past > c.tagInfo.startIndex) {
                if (c.actor.Next) {
                    c.actor.Next = false;
                    // Only skip when there is no "no-skip" attribute.
                    if (!c.tagInfo.attrs.ContainsKey("no-skip")) {
                        c.actor.pi.past = c.tagInfo.endIndex;
                    }
                }
            }
        }
    }
}
