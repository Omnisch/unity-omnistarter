// author: Omnistudio
// version: 2025.11.28

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
        public static async Task<string> TextGeneration(string apiKey, string model, string sysPrompt, string userPrompt) {
            var request = new Request
            {
                model = model,
                input = new Request.Input[] {
                    new() {
                        role = "developer",
                        content = new Request.Input.Content[] {
                            new() {
                                type = "input_text",
                                text = sysPrompt
                            }
                        }
                    },
                    new() {
                        role = "user",
                        content = new Request.Input.Content[] {
                            new() {
                                type = "input_text",
                                text = userPrompt
                            }
                        }
                    }
                }
            };
            var requestString = JsonUtility.ToJson(request);
            var responseRaw = await HttpHelper.PostJsonAsync(BaseUrl, $"Bearer {apiKey}", requestString);
            var response = JsonUtility.FromJson<ChatCompletionResponse>(Encoding.UTF8.GetString(responseRaw));
            Debug.Log($"{response.output[0].status}");
            return response.output[0].content[0].text;
        }

        /// <returns>response.output[0].content[0].text</returns>
        public static async Task<string> AnalyseImage(string apiKey, string model, string prompt, byte[] original) {
            string originalBase64 = Convert.ToBase64String(original);
            var request = new Request
            {
                model = model,
                input = new Request.Input[] {
                    new() {
                        role = "user",
                        content = new Request.Input.Content[] {
                            new() {
                                type = "input_text",
                                text = prompt
                            },
                            new() {
                                type = "input_image",
                                image_url = originalBase64
                            }
                        }
                    }
                }
            };
            var requestString = JsonUtility.ToJson(request);
            var responseRaw = await HttpHelper.PostJsonAsync(BaseUrl, $"Bearer {apiKey}", requestString);
            var response = JsonUtility.FromJson<ChatCompletionResponse>(Encoding.UTF8.GetString(responseRaw));
            Debug.Log($"{response.output[0].status}");
            return response.output[0].content[0].text;
        }

        #region Structs
        [Serializable]
        private class Request
        {
            public string model;
            public Input[] input;

            [Serializable]
            public class Input
            {
                public string role;
                public Content[] content;

                [Serializable]
                public class Content
                {
                    public string type;
                    public string text;
                    public string image_url;
                }
            }
        }

        [Serializable]
        private class ChatCompletionResponse
        {
            public string id;
            public Output[] output;

            [Serializable]
            public class Output
            {
                public string id;
                public string type;
                public string status;
                public Content[] content;
                public string role;

                [Serializable]
                public class Content
                {
                    public string type;
                    public string text;
                }
            }
        }
        #endregion
    }
}
