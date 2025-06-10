// author: Omnistudio
// version: 2025.06.11

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Omnis.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))][DisallowMultipleComponent]
    public class TextActor : PointerBase
    {
        #region Serialized Fields
        public string actorId;
        [SerializeField] private string staticOpeningTags;
        #endregion

        #region Fields
        private TextMeshProUGUI tmpro;
        #endregion

        #region Properties
        public TextMeshProUGUI TMPro => tmpro;
        public float PrintPast { get; set; }
        public float PrintSpeed { get; set; }
        /// <summary>TextActor won't render rich text when using RawLine instead of Line.</summary>
        public string RawLine { set => tmpro.text = value; }
        public string Line
        {
            set
            {
                StopAllCoroutines();
                PrintPast = 0f;
                PrintSpeed = TextManager.DefaultPrintSpeed;
                StartCoroutine(ShowLine(value));
            }
        }
        #endregion

        #region Methods
        private IEnumerator ShowLine(string line)
        {
            var tagInfoList = ParseRichText(line, out string textVisible);
            tmpro.SetText(textVisible);

            if (tagInfoList.Count == 0) yield break;

            while (true)
            {
                if (tagInfoList.All(tagInfo => tagInfo.finished))
                    yield break;

                // Refresh all infos.
                tmpro.ForceMeshUpdate();
                var textInfo = tmpro.textInfo;

                // Perform rich text effects.
                foreach (var tagInfo in tagInfoList)
                {
                    if (tagInfo.finished) continue;
                    var tag = TextManager.Instance.StyleSheet.Tags.Find((tag) => tag.name == tagInfo.name);
                    for (int i = tagInfo.startIndex; i < tagInfo.endIndex; i++)
                        tag?.Tune(new(this, tagInfo, i, Time.time, Input.mousePosition));
                }

                // Update geometry.
                for (int i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    var meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    meshInfo.mesh.colors32 = meshInfo.colors32;
                    tmpro.UpdateGeometry(meshInfo.mesh, i);
                }

                PrintPast += PrintSpeed * Time.deltaTime;
                yield return null;
            }
        }

        private static readonly Regex openTag = new(@"<(?<name>\w+)(?<a>[^>]*?)>", RegexOptions.Compiled);
        private static readonly Regex closeTag = new(@"</(?<name>\w+)\s*>", RegexOptions.Compiled);
        private static List<TagInfo> ParseRichText(string src, out string srcVisible)
        {
            var infos = new List<TagInfo>();
            var stack = new Stack<(string tag, Dictionary<string, string> attr, int start)>();
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
                        infos.Add(new TagInfo { name = n, attrs = a, startIndex = s, endIndex = visibleIndex });

                    i += mClose.Length;
                    continue;
                }

                var mOpen = openTag.Match(src, i);
                if (mOpen.Success && mOpen.Index == i)
                {
                    string n = mOpen.Groups["name"].Value;
                    // parse attributes
                    string a = mOpen.Groups["a"].Value.Trim(' ');
                    var la = a.Split(" ");
                    var da = new Dictionary<string, string>();
                    foreach (string entry in la)
                    {
                        var p = entry.Split('=');
                        if (p.Length > 1)
                            da.Add(p[0], p[1]);
                        else
                            da.Add(entry, "");
                    }
                    // end parse
                    stack.Push((n, da, visibleIndex));

                    i += mOpen.Length;
                    continue;
                }

                srcVisible += src[i];
                visibleIndex++;
                i++;
            }

            while (stack.Count > 0)
            {
                var (openTag, a, s) = stack.Pop();
                infos.Add(new TagInfo { name = openTag, attrs = a, startIndex = s, endIndex = visibleIndex });
            }

            return infos;
        }
        #endregion

        #region Unity Methods
        protected override void Start()
        {
            base.Start();

            tmpro = GetComponent<TextMeshProUGUI>();
            if (TextManager.Instance != null)
                TextManager.Instance.actors.Add(this);
            Line = tmpro.text;
        }

        private void OnDestroy()
        {
            if (TextManager.Instance != null)
                TextManager.Instance.actors.Remove(this);
        }
        #endregion
    }
}
