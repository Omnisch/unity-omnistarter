// author: Omnistudio
// version: 2025.05.04

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Omnis.UI
{
    public abstract class ScriptableStyleSheet : ScriptableObject, IEnumerable<RichTextScriptableTag>
    {
        public abstract List<RichTextScriptableTag> Tags { get; }
        public static implicit operator List<RichTextScriptableTag>(ScriptableStyleSheet sss) => sss.Tags;
        public IEnumerator<RichTextScriptableTag> GetEnumerator() => Tags.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Stores a scriptable TMPro tag, used in <i>ScriptableStyleSheet</i> and <i>TextActor</i>.
    /// </summary>
    public class RichTextScriptableTag
    {
        /// <summary>
        /// The reference used in tags, e.g. &lt;name&gt;&lt;/name&gt;<br/>
        /// It should not be the same with tags preserved by Unity.
        /// </summary>
        public string name;
        public float delta;
        /// <summary>
        /// TMP_Text: The TMPro to be tagged.<br/>
        /// TagInfo: The info of the active tag.<br/>
        /// float: Time after the text been displayed.
        /// </summary>
        public Action<TMP_Text, TagInfo, float> Tune;
    }

    public struct TagInfo
    {
        public string name;
        public string attr;
        public int startIndex;
        public int endIndex;
    }
}
