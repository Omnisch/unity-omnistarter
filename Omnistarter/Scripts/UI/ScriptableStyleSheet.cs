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
    [Serializable]
    public struct RichTextScriptableTag
    {
        /// <summary>
        /// The reference used in tags, e.g. &lt;id&gt;&lt;/id&gt;<br/>
        /// It should not be the same with tags preserved by Unity.
        /// </summary>
        public string id;
        public bool applyToCharWithPhase;
        public string OpeningTag { private get; set; }
        public string ClosingTag { get; set; }
        /// <summary>The function to tune the raw tags.</summary>
        public Func<string, float, string> tuneFunc;
        public readonly string TunedOpeningTag(float phase) => tuneFunc(OpeningTag, phase);
    }
}
