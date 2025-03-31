// author: Omnistudio
// version: 2025.03.31

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.UI
{
    public abstract class ScriptableStyleSheet : ScriptableObject
    {
        public abstract List<RichTextScriptableTag> Tags { get; }
    }

    /// <summary>
    /// Stores a scriptable TMP tag, used in <i>ScriptableStyleSheet</i>, <i>TextActor</i> and <i>TextManager</i>.
    /// </summary>
    public struct RichTextScriptableTag
    {
        /// <summary>
        /// The reference used in tags, e.g. &lt;name&gt;&lt;/name&gt;<br/>
        /// It should not be the same with tags preserved by Unity.
        /// </summary>
        public string name;
        public string OpeningTag { private get; set; }
        public string ClosingTag { get; set; }
        /// <summary>The type of param which <i>TextActor</i> will pass.</summary>
        public ScriptableTagParamType paramType;
        /// <summary>The length of spectrum, only available when <i>paramType</i> contains Spectrum.</summary>
        public float spectrumLength;
        /// <summary>The delta value, only available when <i>paramType</i> contains Delta.</summary>
        public float delta;
        /// <summary>The function to tune the raw tags.</summary>
        public Func<string, float, string> tuneFunc;
        public readonly string TunedOpeningTag(float param) => tuneFunc(OpeningTag, param);
    }

    public enum ScriptableTagParamType
    {
        One = 1,
        Time = 2,
        Spectrum = 4,
        Delta = 8,
        TimeWithSpectrum = Time | Spectrum,
        TimeWithDelta = Time | Delta,
    }
}
