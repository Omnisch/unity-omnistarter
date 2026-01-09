// author: Omnistudio
// version: 2026.01.09

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Omnis.Utils;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Omnis.API
{
    public static class OpenAiApi
    {
        private static readonly string BaseUrl = "https://api.openai.com/v1/responses";

        /// <returns>response.output[0].content[0].text</returns>
        public static async Task<string> TextGeneration(string apiKey, string model, string userPrompt, string sysPrompt = null, object textFormat = null) {
            var request = new {
                model,
                input = new object[] {
                    string.IsNullOrEmpty(sysPrompt) ? new { } : new {
                        role = "developer",
                        content = new object[] {
                            new {
                                type = "input_text",
                                text = sysPrompt
                            }
                        }
                    },
                    new {
                        role = "user",
                        content = new object[] {
                            new {
                                type = "input_text",
                                text = userPrompt
                            }
                        }
                    }
                },
                text = new {
                    format = textFormat?? new { },
                }
            };
            var requestString = JsonConvert.SerializeObject(request);
            var responseRaw = await HttpHelper.PostJsonAsync(BaseUrl, $"Bearer {apiKey}", requestString);
            var response = JObject.Parse(Encoding.UTF8.GetString(responseRaw));

            Debug.Log($"{response["output"]?[0]?["status"]}");

            var content0 = response["output"]?[0]?["content"]?[0];
            if (content0 == null) {
                throw new Exception("No output content.");
            }

            var type = content0.Value<string>("type");
            if (type == "refusal") {
                throw new Exception("Model refusal: " + content0.Value<string>("refusal"));
            }

            var text = content0.Value<string>("text");
            if (string.IsNullOrEmpty(text)) {
                throw new Exception("Empty output text.");
            }

            return text;
        }

        /// <returns>response.output[0].content[0].text</returns>
        public static async Task<string> AnalyseImage(string apiKey, string model, string prompt, byte[] original) {
            string originalBase64 = Convert.ToBase64String(original);
            var request = new {
                model,
                input = new object[] {
                    new {
                        role = "user",
                        content = new object[] {
                            new {
                                type = "input_text",
                                text = prompt
                            },
                            new {
                                type = "input_image",
                                image_url = originalBase64
                            }
                        }
                    }
                }
            };
            var requestString = JsonConvert.SerializeObject(request);
            var responseRaw = await HttpHelper.PostJsonAsync(BaseUrl, $"Bearer {apiKey}", requestString);
            var response = JObject.Parse(Encoding.UTF8.GetString(responseRaw));

            Debug.Log($"{response["output"]?[0]?["status"]}");

            var content0 = response["output"]?[0]?["content"]?[0];
            if (content0 == null) {
                throw new Exception("No output content.");
            }

            var type = content0.Value<string>("type");
            if (type == "refusal") {
                throw new Exception("Model refusal: " + content0.Value<string>("refusal"));
            }

            var text = content0.Value<string>("text");
            if (string.IsNullOrEmpty(text)) {
                throw new Exception("Empty output text.");
            }

            return text;
        }
    }
}
