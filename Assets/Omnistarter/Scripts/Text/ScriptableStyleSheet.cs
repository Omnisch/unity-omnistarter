// author: Omnistudio
// version: 2025.07.06

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Omnis.Text
{
    public abstract class ScriptableStyleSheet : ScriptableObject
    {
        public abstract List<ScriptableRichTextTag> Tags { get; }
        public static implicit operator List<ScriptableRichTextTag>(ScriptableStyleSheet sss) => sss.Tags;
        public IEnumerator<ScriptableRichTextTag> GetEnumerator() => Tags.GetEnumerator();
        public ScriptableRichTextTag Find(Predicate<ScriptableRichTextTag> match) => Tags.Find(match);

        /// <summary>
        /// Stores a scriptable TMPro tag, used in <i>ScriptableStyleSheet</i> and <i>TextActor</i>.
        /// </summary>
        public class ScriptableRichTextTag
        {
            /// <summary>
            /// The reference used in tags, e.g. &lt;name&gt;&lt;/name&gt; or &lt;name /&gt;<br/>
            /// It should not be the same with tags preserved by Unity.
            /// </summary>
            public string name;
            /// <summary>
            /// The function to render texts.<br/>
            /// CharInfo: The character to be rendered.
            /// </summary>
            public Action<CharInfo> render;

            public ScriptableRichTextTag(string name, Action<CharInfo> render)
            {
                this.name = name;
                this.render = render;
            }
        }
    }

    /// <summary>
    /// Stores the info of a scriptable tag instance.<br/>
    /// Will be created in <i>TextActor.ParseRichText()</i> and used in <i>CharInfo</i>.
    /// </summary>
    public class TagInfo
    {
        public string name = "";
        public Dictionary<string, string> attrs = null;
        public int startIndex = 0;
        public int endIndex = 0;
        public bool active = false;
        public bool finished = false;
    }

    /// <summary>
    /// Runtime info of one character in a TMPro Text.<br/>
    /// Will be created in <i>TextActor.ShowLine()</i> and used in <i>ScriptableRichTextTag</i>.
    /// </summary>
    public struct CharInfo
    {
        public readonly TextActor actor;
        public TagInfo tagInfo;
        public readonly int index;

        // Runtime params.
        public readonly float time;
        public readonly Vector3 mousePosition;

        public CharInfo(TextActor actor, TagInfo tagInfo, int index, float time, Vector3 mousePosition)
        {
            this.actor = actor;
            this.tagInfo = tagInfo;
            this.index = index;
            this.time = time;
            this.mousePosition = mousePosition;
        }
    }

    public static class CharInfoExtensions
    {
        public static float Spectrum(this CharInfo c)
            => c.tagInfo.endIndex <= c.tagInfo.startIndex + 1 ? 0f : (c.index - c.tagInfo.startIndex) / (float)(c.tagInfo.endIndex - c.tagInfo.startIndex - 1);
        public static CharInfo SimpleEditVertices(this CharInfo c, Vector3 value)
        {
            var charInfo = c.actor.TMPro.textInfo.characterInfo[c.index];
            if (!charInfo.isVisible) return c;
            int matIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;
            Vector3[] vertices = c.actor.TMPro.textInfo.meshInfo[matIndex].vertices;
            for (int i = 0; i < 4; i++)
                vertices[vertexIndex + i] += value;
            return c;
        }
        public static CharInfo SimpleEditColor(this CharInfo c, Color32 value)
        {
            var charInfo = c.actor.TMPro.textInfo.characterInfo[c.index];
            if (!charInfo.isVisible) return c;
            int matIndex = charInfo.materialReferenceIndex;
            Color32[] colors32 = c.actor.TMPro.textInfo.meshInfo[matIndex].colors32;
            int vertexIndex = charInfo.vertexIndex;
            for (int i = 0; i < 4; i++)
                colors32[vertexIndex + i] = value;
            return c;
        }
        /// <summary>Alpha is float 0 ~ 1.</summary>
        public static CharInfo SimpleEditAlpha(this CharInfo c, float value)
        {
            var charInfo = c.actor.TMPro.textInfo.characterInfo[c.index];
            if (!charInfo.isVisible) return c;
            int matIndex = charInfo.materialReferenceIndex;
            Color32[] colors32 = c.actor.TMPro.textInfo.meshInfo[matIndex].colors32;
            int vertexIndex = charInfo.vertexIndex;
            for (int i = 0; i < 4; i++)
                colors32[vertexIndex + i].a = (byte)(255 * value);
            return c;
        }
    }
}
