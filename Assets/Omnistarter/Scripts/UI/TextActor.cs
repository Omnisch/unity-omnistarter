// author: Omnistudio
// version: 2025.05.04

using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Omnis.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))][DisallowMultipleComponent]
    public class TextActor : MonoBehaviour
    {
        #region Serialized Fields
        public string actorId;
        public string staticOpeningTags;
        #endregion

        #region Fields
        private TextMeshProUGUI tmpro;
        private float lineStartTime;
        #endregion

        #region Properties
        /// <summary>Set TMPro text directly.</summary>
        public string RawLine { set => tmpro.text = value; }
        public string Line
        {
            set
            {
                StopAllCoroutines();
                lineStartTime = Time.time;
                StartCoroutine(ShowLine(value));
            }
        }
        #endregion

        #region Methods
        private IEnumerator ShowLine(string line)
        {
            var tagInfoList = ParseRichText(line, out string textVisible);
            tmpro.SetText(textVisible);

            while (true)
            {
                // Refresh all infos.
                tmpro.ForceMeshUpdate();

                // Perform rich text effects.
                foreach (var tagInfo in tagInfoList)
                {
                    var tag = TextManager.Instance.StyleSheet.Tags.Find((tag) => tag.name == tagInfo.name);
                    tag?.Tune(tmpro, tagInfo, Time.time - lineStartTime);
                }

                // Update geometry.
                var textInfo = tmpro.textInfo;
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    var meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    meshInfo.mesh.colors32 = meshInfo.colors32;
                    tmpro.UpdateGeometry(meshInfo.mesh, i);
                }

                yield return null;
            }
        }

        private static readonly Regex openTag = new(@"<(?<name>\w+)(?<a>[^>]*?)>", RegexOptions.Compiled);
        private static readonly Regex closeTag = new(@"</(?<name>\w+)\s*>", RegexOptions.Compiled);
        private static List<TagInfo> ParseRichText(string src, out string srcVisible)
        {
            var infos = new List<TagInfo>();
            var stack = new Stack<(string tag, string attr, int start)>();
            srcVisible = "";

            int visibleIndex = 0;
            for (int i = 0; i < src.Length;)
            {
                var mClose = closeTag.Match(src, i);
                if (mClose.Success && mClose.Index == i)
                {
                    string n = mClose.Groups["name"].Value;
                    var (openTag, a, s) = stack.Pop();
                    if (openTag == n)
                        infos.Add(new TagInfo { name = n, attr = a, startIndex = s, endIndex = visibleIndex });

                    i += mClose.Length;
                    continue;
                }

                var mOpen = openTag.Match(src, i);
                if (mOpen.Success && mOpen.Index == i)
                {
                    string n = mOpen.Groups["name"].Value;
                    var a = mOpen.Groups["a"].Value.Trim();
                    stack.Push((n, a, visibleIndex));

                    i += mOpen.Length;
                    continue;
                }

                srcVisible += src[i];
                visibleIndex++;
                i++;
            }
            return infos;
        }
        #endregion

        #region Unity Methods
        private void Start()
        {
            tmpro = GetComponent<TextMeshProUGUI>();
            if (TextManager.Instance != null)
                TextManager.Instance.actors.Add(this);
        }

        private void OnDestroy()
        {
            if (TextManager.Instance != null)
                TextManager.Instance.actors.Remove(this);
        }
        #endregion
    }
}
