// author: Omnistudio
// version: 2025.03.30

using System.Collections.Generic;

namespace Omnis.UI
{
    public struct RichTextLine
    {
        public string text;
        public List<RichTextScriptableTag> tags;
    }

    /// <summary>
    /// Stores a scriptable TMP tag, used in <i>RichTextLine</i>, <i>TextActor</i> and <i>TextManager</i>.
    /// </summary>
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
        public System.Func<string, float, string> tuneFunc;
        public readonly string TunedOpeningTag(float phase) => tuneFunc(OpeningTag, phase);
    }
}
