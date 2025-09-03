// author: Omnistudio
// version: 2025.09.03

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Omnis.Text
{
    [RequireComponent(typeof(TMP_Text))]
    [DisallowMultipleComponent]
    public class TextActor : MonoBehaviour
    {
        #region Serialized Fields
        public string actorId;
        [SerializeField] private string staticHead;
        public static readonly float DefaultPrintSpeed = 20f;
        [Header("Animation Scale")]
        [SerializeField][Range(-1, 1)] private float animScale = 1f;
        [SerializeField] private bool isWorldPos;
        #endregion

        #region Fields
        private TMP_Text tmpro;
        public PrintInfo pi;
        public float AnimFactor => this.isWorldPos ? this.animScale : this.animScale * 50f;
        #endregion

        #region Properties
        public TMP_Text TMPro => this.tmpro;
        /// <summary>TextActor won't render rich text when using RawLine instead of Line.</summary>
        public string RawLine
        {
            set {
                this.StopAllCoroutines();
                this.tmpro.text = value;
            }
        }
        public string Line
        {
            set {
                this.StopAllCoroutines();
                this.StartCoroutine(this.ShowLine(this.staticHead + value));
            }
        }
        private bool next;
        public bool Next
        {
            get => this.next;
            set {
                this.next = value;
                if (value) {
                    IEnumerator ResetNextTrigger()
                    {
                        yield return 0;
                        yield return new WaitForEndOfFrame();
                        if (this.next) {
                            TextManager.Instance.NextLine(callFromActor: this.actorId);
                            this.next = false;
                        }
                    }

                    this.StartCoroutine(ResetNextTrigger());
                }
            }
        }
        #endregion

        #region Methods
        private IEnumerator ShowLine(string line)
        {
            this.pi = new();

            var tagInfoList = ParseRichText(line, out string textVisible);
            this.tmpro.SetText(textVisible);
            
            if (tagInfoList.Count == 0) yield break;

            while (!tagInfoList.All(tagInfo => tagInfo.finished)) {
                // Refresh all infos.
                this.tmpro.ForceMeshUpdate();
                var textInfo = this.tmpro.textInfo;

                // Perform rich text effects.
                foreach (var tagInfo in tagInfoList) {
                    if (tagInfo.finished) continue;

                    var tag = TextManager.Instance.StyleSheet.Find((tag) => tag.name == tagInfo.name);
                    for (int i = tagInfo.startIndex; i < tagInfo.endIndex; i++) {
                        tag?.render(new CharInfo(this, tagInfo, i, Time.time, Input.mousePosition));
                    }
                }

                // Update geometry.
                for (int i = 0; i < textInfo.meshInfo.Length; i++) {
                    var meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    meshInfo.mesh.colors32 = meshInfo.colors32;
                    this.tmpro.UpdateGeometry(meshInfo.mesh, i);
                }

                if (!this.pi.pause) {
                    this.pi.past += this.pi.speed * Time.deltaTime;
                }

                yield return null;
            }
        }


        private static readonly Regex openTag = new(@"<(?<name>\w+)(?<attrs>[^>]*?)>", RegexOptions.Compiled);
        private static readonly Regex closeTag = new(@"</(?<name>\w+)\s*>", RegexOptions.Compiled);
        private static List<TagInfo> ParseRichText(string src, out string srcVisible)
        {
            var result = new List<TagInfo>();
            var tmpList = new List<TagInfo>();
            srcVisible = "";

            int visibleIndex = 0;
            for (int i = 0; i < src.Length;) {
                var mClose = closeTag.Match(src, i);
                if (mClose.Success && mClose.Index == i) {
                    string n = mClose.Groups["name"].Value;
                    var open = tmpList.Find((tag) => tag.name == n);

                    // open tag found.
                    if (!Equals(open, null)) {
                        open.endIndex = visibleIndex;
                        result.Add(open);
                        tmpList.Remove(open);

                        i += mClose.Length;
                        continue;
                    }
                    // open tag not found, this is a bare close tag, just skip it.
                    else {
                        srcVisible += src[mClose.Index..(mClose.Index + mClose.Length)];
                        visibleIndex += mClose.Length;
                        i += mClose.Length;
                    }
                }

                var mOpen = openTag.Match(src, i);
                if (mOpen.Success && mOpen.Index == i) {
                    string n = mOpen.Groups["name"].Value;

                    // the tag is a custom tag.
                    if (TextManager.Instance.StyleSheet.Tags.Exists(tag => tag.name == n)) {
                        // Parse attributes.
                        string a = mOpen.Groups["attrs"].Value.Trim(' ');
                        var open = new TagInfo
                        {
                            name = n,
                            attrs = new(),
                            startIndex = visibleIndex
                        };

                        var la = a.Split(" ");
                        foreach (string entry in la) {
                            if (entry != "/") {
                                var p = entry.Split('=');
                                open.attrs.Add(p[0], p.Length > 1 ? string.Join("=", p[1..]) : "");
                            }
                        }

                        // Isolated tags, such as "<br />".
                        if (mOpen.Value[^2] == '/') {
                            open.endIndex = visibleIndex + 1;
                            result.Add(open);
                            // Add a zero width space for any iso tags, so that it won't affect other characters.
                            srcVisible += '\u200B';
                            visibleIndex++;
                        }
                        else {
                            tmpList.Add(open);
                        }

                        i += mOpen.Length;
                        continue;
                    }
                    // the tag is not a custom tag, maybe a Unity-preserved tag. skip it.
                    else {
                        srcVisible += src[mOpen.Index..(mOpen.Index + mOpen.Length)];
                        visibleIndex += mOpen.Length;
                        i += mOpen.Length;
                    }
                }

                srcVisible += src[i];
                visibleIndex++;
                i++;
            }

            foreach (var orphan in tmpList) {
                orphan.endIndex = visibleIndex;
                result.Add(orphan);
            }

            return result;
        }
        #endregion

        #region Unity Methods
        private void Start()
        {
            this.tmpro = this.GetComponent<TMP_Text>();
            TextManager.Instance.AddActor(this);
        }

        private void OnDestroy()
        {
            if (TextManager.Instance != null)
                TextManager.Instance.RemoveActor(this);
        }
        #endregion

        #region Structs
        public class PrintInfo
        {
            public float past;
            public float speed;
            public bool pause;
            public PrintInfo()
            {
                this.past = 0;
                this.speed = DefaultPrintSpeed;
                this.pause = false;
            }
        }
        #endregion
    }
}
