// author: Omnistudio
// version: 2025.05.05

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
        /// <summary>
        /// TMP_Text: The TMPro to be tagged.<br/>
        /// TagInfo: The info of the active tag.<br/>
        /// float: Time after the text been displayed.
        /// </summary>
        public Action<CharInfo> Tune;

        public RichTextScriptableTag(string name, Action<CharInfo> tune)
        {
            this.name = name;
            Tune = tune;
        }
    }

    public struct TagInfo
    {
        public string name;
        public int startIndex;
        public int endIndex;
    }

    public readonly struct CharInfo
    {
        public readonly TMP_Text tmpro;
        public readonly TagInfo tagInfo;
        public readonly int index;
        public readonly float time;

        public CharInfo(TMP_Text tmpro, TagInfo tagInfo, int index, float time)
        {
            this.tmpro = tmpro;
            this.tagInfo = tagInfo;
            this.index = index;
            this.time = time;
        }
    }
}
