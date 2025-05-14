// author: Omnistudio
// version: 2025.05.14

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Omnis.UI
{
    public abstract class ScriptableStyleSheet : ScriptableObject, IEnumerable<ScriptableRichTextTag>
    {
        public abstract List<ScriptableRichTextTag> Tags { get; }
        public static implicit operator List<ScriptableRichTextTag>(ScriptableStyleSheet sss) => sss.Tags;
        public IEnumerator<ScriptableRichTextTag> GetEnumerator() => Tags.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    /// <summary>
    /// Stores a scriptable TMPro tag, used in <i>ScriptableStyleSheet</i> and <i>TextActor</i>.
    /// </summary>
    public class ScriptableRichTextTag
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

        public ScriptableRichTextTag(string name, Action<CharInfo> tune)
        {
            this.name = name;
            Tune = tune;
        }
    }

    /// <summary>
    /// Stores the info of a scriptable tag instance, will be created in <i>TextActor</i> and used in <i>CharInfo</i>.
    /// </summary>
    public struct TagInfo
    {
        public string name;
        public Dictionary<string, string> attrs;
        public int startIndex;
        public int endIndex;
    }

    public readonly struct CharInfo
    {
        public readonly TMP_Text tmpro;
        public readonly TagInfo tagInfo;
        public readonly int index;

        // Game params.
        public readonly float time;
        public readonly Vector3 mousePosition;

        public CharInfo(TMP_Text tmpro, TagInfo tagInfo, int index, float time, Vector3 mousePosition)
        {
            this.tmpro = tmpro;
            this.tagInfo = tagInfo;
            this.index = index;
            this.time = time;
            this.mousePosition = mousePosition;
        }
    }
}
