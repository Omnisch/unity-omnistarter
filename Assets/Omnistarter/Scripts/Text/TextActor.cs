// author: Omnistudio
// version: 2026.01.02

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
        public float AnimFactor => isWorldPos ? animScale : animScale * 50f;
        #endregion


        #region Properties
        public TMP_Text TMPro => tmpro;
        /// <summary>TextActor won't render rich text when using RawLine instead of Line.</summary>
        public string RawLine {
            set {
                StopAllCoroutines();
                tmpro.text = value;
            }
        }
        public string Line {
            set {
                StopAllCoroutines();
                StartCoroutine(ShowLine(staticHead + value));
            }
        }

        private bool next;
        public bool Next {
            get => next;
            set {
                next = value;
                if (value) {
                    IEnumerator ResetNextTrigger() {
                        yield return 0;
                        yield return new WaitForEndOfFrame();
                        if (next) {
                            DialogManager.Instance.NextLine();
                            next = false;
                        }
                    }

                    StartCoroutine(ResetNextTrigger());
                }
            }
        }
        #endregion


        #region Methods
        private IEnumerator ShowLine(string line) {
            pi = new();

            var tagInfoList = ParseRichText(line, out string textVisible);
            tmpro.SetText(textVisible);

            if (tagInfoList.Count == 0) yield break;

            while (!tagInfoList.All(tagInfo => tagInfo.finished)) {
                // Refresh all infos.
                tmpro.ForceMeshUpdate();
                var textInfo = tmpro.textInfo;

                // Perform rich text effects.
                foreach (var tagInfo in tagInfoList) {
                    if (tagInfo.finished) continue;

                    var tag = DialogManager.Instance.StyleSheet.Find((tag) => tag.name == tagInfo.name);
                    for (int i = tagInfo.startIndex; i < tagInfo.endIndex; i++) {
                        tag?.render(new CharInfo(this, tagInfo, i, Time.time, Input.mousePosition));
                    }
                }

                // Update geometry.
                for (int i = 0; i < textInfo.meshInfo.Length; i++) {
                    var meshInfo = textInfo.meshInfo[i];
                    meshInfo.mesh.vertices = meshInfo.vertices;
                    meshInfo.mesh.colors32 = meshInfo.colors32;
                    tmpro.UpdateGeometry(meshInfo.mesh, i);
                }

                if (!pi.pause) {
                    pi.past += pi.speed * Time.deltaTime;
                }

                yield return null;
            }
        }


        private static readonly Regex openTag = new(@"<(?<name>\w+)(?<attrs>[^>]*?)>", RegexOptions.Compiled);
        private static readonly Regex closeTag = new(@"</(?<name>\w+)\s*>", RegexOptions.Compiled);
        private static List<TagInfo> ParseRichText(string src, out string srcVisible) {
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
                    if (DialogManager.Instance.StyleSheet.Tags.Exists(tag => tag.name == n)) {
                        // Parse attributes.
                        string a = mOpen.Groups["attrs"].Value.Trim(' ');
                        var open = new TagInfo {
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
                        } else {
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
        private void Start() {
            tmpro = GetComponent<TMP_Text>();
            DialogManager.Instance.AddActor(this);
        }

        private void OnDestroy() {
            if (DialogManager.Instance != null)
                DialogManager.Instance.RemoveActor(this);
        }
        #endregion


        #region Structs
        public class PrintInfo
        {
            public float past;
            public float speed;
            public bool pause;
            public PrintInfo() {
                past = 0;
                speed = DefaultPrintSpeed;
                pause = false;
            }
        }
        #endregion
    }
}
