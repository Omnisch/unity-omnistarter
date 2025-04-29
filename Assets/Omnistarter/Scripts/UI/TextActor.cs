// author: Omnistudio
// version: 2025.03.31

using Omnis.Utils;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Omnis.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextActor : MonoBehaviour
    {
        #region Serialized Fields
        public string actorName;
        public string staticOpeningTags;
        #endregion

        #region Fields
        private TextMeshProUGUI tmpro;
        private string line;
        private float lineStartTime;
        #endregion

        #region Properties
        public string RawLine
        {
            set => tmpro.text = value;
        }
        public string Line
        {
            set
            {
                StopAllCoroutines();
                line = value;
                lineStartTime = Time.time;
                StartCoroutine(ShowLine());
            }
        }
        #endregion

        #region Methods
        private IEnumerator ShowLine()
        {
            while (true)
            {
                string text = line;
                foreach (var tag in TextManager.Instance.StyleSheet.Tags)
                {
                    float param = tag.paramType == ScriptableTagParamType.One ? 1f : 0f;

                    if ((tag.paramType & ScriptableTagParamType.Time) != 0)
                        param = Time.time - lineStartTime;

                    if ((tag.paramType & ScriptableTagParamType.Spectrum) != 0f)
                    {
                        Regex reg = new($@"<{tag.name}>(.*?)</{tag.name}>", RegexOptions.Singleline);
                        text = reg.Replace(text, (match) =>
                        {
                            string result = staticOpeningTags;
                            string capture = match.Groups[1].Value;
                            bool skipOtherTags = false;
                            for (int i = 0; i < capture.Length; i++)
                            {
                                if (capture[i] == '<') skipOtherTags = true;
                                if (skipOtherTags)
                                {
                                    if (capture[i] == '>') skipOtherTags = false;
                                    result += capture[i];
                                    continue;
                                }
                                result += string.Join("",
                                    tag.TunedOpeningTag(param + Mathf.Lerp(0f, tag.spectrumLength, i / (float)(capture.Length - 1))), capture[i], tag.ClosingTag
                                );
                            }
                            return result;
                        });
                    }
                    else if ((tag.paramType & ScriptableTagParamType.Delta) != 0f)
                    {
                        Regex reg = new($@"<{tag.name}>(.*?)</{tag.name}>", RegexOptions.Singleline);
                        text = reg.Replace(text, (match) =>
                        {
                            string result = staticOpeningTags;
                            string capture = match.Groups[1].Value;
                            bool skipOtherTags = false;
                            float phase = 0f;
                            for (int i = 0; i < capture.Length; i++)
                            {
                                if (capture[i] == '<') skipOtherTags = true;
                                if (skipOtherTags)
                                {
                                    if (capture[i] == '>') skipOtherTags = false;
                                    result += capture[i];
                                    continue;
                                }
                                result += string.Join("",
                                    tag.TunedOpeningTag(param + phase), capture[i], tag.ClosingTag
                                );
                                phase += tag.delta;
                            }
                            return result;
                        });
                    }
                    else
                    {
                        text = text.Replace($"<{tag.name}>", tag.TunedOpeningTag(param))
                                   .Replace($"</{tag.name}>", tag.ClosingTag);
                    }
                }
                tmpro.text = text;
                yield return null;
            }
        }
        #endregion

        #region Unity Methods
        private void Start()
        {
            tmpro = GetComponent<TextMeshProUGUI>();
            var manager = FindFirstObjectByType<TextManager>();
            if (manager != null)
            {
                manager.actors.Add(this);
            }
        }

        private void OnDestroy()
        {
            var manager = FindFirstObjectByType<TextManager>();
            if (manager != null)
            {
                manager.actors.Remove(this);
            }
        }
        #endregion
    }
}
