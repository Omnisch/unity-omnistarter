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
        [Range(-1f, 1f)] public float phaseDelta = -0.03f;
        #endregion

        #region Fields
        private TextMeshProUGUI tmpro;
        public RichTextLine line;
        #endregion

        #region Properties
        public string RawText
        {
            set => tmpro.text = value;
        }
        public RichTextLine Line
        {
            set
            {
                StopAllCoroutines();
                line = value;
                StartCoroutine(ShowLine());
            }
        }
        #endregion

        #region Methods
        private IEnumerator ShowLine()
        {
            while (true)
            {
                string text = line.text;
                foreach (var tag in line.tags)
                {
                    if (tag.applyToCharWithPhase)
                    {
                        Regex reg = new($@"<{tag.id}>(.*?)</{tag.id}>", RegexOptions.Singleline);
                        text = reg.Replace(text, (match) =>
                        {
                            string result = "";
                            string capture = match.Groups[1].Value;
                            bool skipOtherTags = false;
                            float phase = 0f;
                            foreach (char c in capture)
                            {
                                if (c == '<') skipOtherTags = true;
                                if (skipOtherTags)
                                {
                                    if (c == '>') skipOtherTags = false;
                                    result += c;
                                    continue;
                                }

                                result += tag.TunedOpeningTag(phase) + c + tag.ClosingTag;
                                phase += phaseDelta;
                            }
                            return result;
                        });
                    }
                    else
                    {
                        text = text.Replace($"<{tag.id}>", tag.TunedOpeningTag(0f))
                                   .Replace($"</{tag.id}>", tag.ClosingTag);
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
