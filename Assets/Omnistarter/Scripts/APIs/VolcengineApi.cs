// author: Omnistudio
// version: 2026.03.05

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Omnis.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Omnis.API
{
    public static class VolcengineApi
    {
        public const string ChatUrl  = "https://ark.cn-beijing.volces.com/api/v3/chat/completions";
        public const string ImageUrl = "https://ark.cn-beijing.volces.com/api/v3/images/generations";
        public const string TTSUrl   = "https://openspeech.bytedance.com/api/v1/tts";
        public const string ASRUrl   = "https://openspeech.bytedance.com/api/v3/auc/bigmodel/recognize/flash";


        #region Chat
        /// <summary>
        /// You can use VolcengineApi.CombineRequest() to easily combine a request structure.<br/>
        /// Chat() technically supports multimodal, but you need to manually build the request object instead of using CombineRequest().
        /// </summary>
        /// <returns>data, an array containing message.content(s)</returns>
        public static async Task<IEnumerable<ChatResponseDataItem>> Chat(string apiKey, object request, Action<string, LogLevel> upstreamLog = null) {
            var requestString = JsonConvert.SerializeObject(request);
            var responseRaw = await HttpHelper.PostJsonAsync(ChatUrl, $"Bearer {apiKey}", requestString, upstreamLog);
            var response = JObject.Parse(Encoding.UTF8.GetString(responseRaw));

            if (response["choices"] is JArray arr) {
                return arr.ToObject<List<ChatResponseDataItem>>();
            } else {
                return Array.Empty<ChatResponseDataItem>();
            }
        }

        /// <param name="thinking">"enabled" / "disabled" / "auto"</param>
        /// <param name="responseSchema">You can use Utils.HttpHelper.CombineJsonSchema() to easily combine a schema.</param>
        public static object CombineRequest(
            ChatModel model,
            string systemPrompt,
            string userPrompt,
            string thinking = "disabled",
            object responseSchema = null) {
            return new {
                model = GetModelID(model),
                messages = new[] {
                    string.IsNullOrEmpty(systemPrompt) ? null : new {
                        role = "system",
                        content = systemPrompt,
                    },
                    string.IsNullOrEmpty(userPrompt) ? null : new {
                        role = "user",
                        content = userPrompt,
                    }
                },
                thinking = new {
                    type = thinking,
                },
                response_format = responseSchema == null ? null : new {
                    type = "json_schema",
                    json_schema = responseSchema,
                },
            };
        }

        public enum ChatModel {
            doubao_seed_2_0_pro,
            doubao_seed_2_0_lite,
            doubao_seed_2_0_mini,
            doubao_seed_2_0_code,
            doubao_seed_1_8,
            glm_4_7,
            deepseek_v3_2,
            kimi_k2,
            doubao_seed_code,
            doubao_seed_1_6_lite,
            deepseek_v3_1_terminus,
            doubao_seed_translation,
            doubao_seed_1_6_flash,
            doubao_seed_1_6_vision,
            deepseek_r1,
        }
        public static string GetModelID(ChatModel m) {
            return m switch {
                ChatModel.doubao_seed_2_0_pro       => "doubao-seed-2-0-pro-260215",
                ChatModel.doubao_seed_2_0_lite      => "doubao-seed-2-0-lite-260215",
                ChatModel.doubao_seed_2_0_mini      => "doubao-seed-2-0-mini-260215",
                ChatModel.doubao_seed_2_0_code      => "doubao-seed-2-0-code-preview-260215",
                ChatModel.doubao_seed_1_8           => "doubao-seed-1-8-251228",
                ChatModel.glm_4_7                   => "glm-4-7-251222",
                ChatModel.deepseek_v3_2             => "deepseek-v3-2-251201",
                ChatModel.kimi_k2                   => "kimi-k2-thinking-251104",
                ChatModel.doubao_seed_code          => "doubao-seed-code-preview-251028",
                ChatModel.doubao_seed_1_6_lite      => "doubao-seed-1-6-lite-251015",
                ChatModel.deepseek_v3_1_terminus    => "deepseek-v3-1-terminus",
                ChatModel.doubao_seed_translation   => "doubao-seed-translation-250915",
                ChatModel.doubao_seed_1_6_flash     => "doubao-seed-1-6-flash-250828",
                ChatModel.doubao_seed_1_6_vision    => "doubao-seed-1-6-vision-250815",
                ChatModel.deepseek_r1               => "deepseek-r1-250528",
                _ => "doubao-seed-1-8-251228"
            };
        }

        [Serializable]
        public struct ChatResponseDataItem {
            public int index;
            public string finish_reason;
            public Message message;
            public struct Message {
                public string content;
                public string reasoning_content;
            }
        }
        #endregion


        #region Image
        /// <summary>You can use VolcengineApi.CombineRequest() to easily combine a request structure.</summary>
        /// <returns>data, an array of { url/b64_json, size }</returns>
        public static async Task<IEnumerable<ImageResponseDataItem>> GenerateImage(string apiKey, object request, Action<string, LogLevel> upstreamLog = null) {
            var requestString = JsonConvert.SerializeObject(request);
            var responseRaw = await HttpHelper.PostJsonAsync(ImageUrl, $"Bearer {apiKey}", requestString, upstreamLog);
            var response = JObject.Parse(Encoding.UTF8.GetString(responseRaw));
            if (response["error"] != null) {
                LogHelper.LogError($"[{response["error"]?["code"]}] {response["error"]?["message"]}", upstreamLog);
            }

            if (response["data"] is JArray arr) {
                return arr.ToObject<List<ImageResponseDataItem>>();
            } else {
                return Array.Empty<ImageResponseDataItem>();
            }
        }

        /// <param name="image">string or string[], each a url or base64</param>
        /// <param name="size">"2K", "3K" or "2048x2048", "2560x1440", etc.</param>
        /// <param name="sequential_image_generation">"auto" / "disabled"</param>
        /// <param name="response_format">"url" / "b64_json"</param>
        public static object CombineRequest(
            ImageModel model,
            string prompt,
            object image = null,
            string size = "2K",
            string sequential_image_generation = "disabled",
            string response_format = "b64_json") {
            return new {
                model = GetModelID(model),
                prompt,
                image,
                size,
                sequential_image_generation,
                response_format,
                watermark = false
            };
        }

        public enum ImageModel {
            doubao_seedream_5_0,
            doubao_seedream_4_5,
            doubao_seedream_4_0,
        }
        public static string GetModelID(ImageModel m) {
            return m switch {
                ImageModel.doubao_seedream_5_0 => "doubao-seedream-5-0-260128",
                ImageModel.doubao_seedream_4_5 => "doubao-seedream-4-5-251128",
                ImageModel.doubao_seedream_4_0 => "doubao-seedream-4-0-250828",
                _ => "doubao-seedream-4-0-250828"
            };
        }

        [Serializable]
        public struct ImageResponseDataItem {
            [JsonProperty("url")]
            public string Url { get; set; }
            [JsonProperty("b64_json")]
            public string Base64 { get; set; }
            [JsonProperty("size")]
            public string Size { get; set; }
        }
        #endregion


        #region Voice
        /// <returns>response.data</returns>
        public static async Task<string> VoiceSynthesis(string appid, string token, string prompt, string voice_type, Action<string, LogLevel> upstreamLog = null) {
            var request = new {
                app = new {
                    appid,
                    token,
                    cluster = "volcano_tts",
                },
                user = new {
                    uid = Application.buildGUID
                },
                audio = new {
                    voice_type,
                },
                request = new {
                    reqid = Guid.NewGuid().ToString(),
                    text = prompt,
                    operation = "query"
                }
            };
            var requestString = JsonConvert.SerializeObject(request);
            var responseRaw = await HttpHelper.PostJsonAsync(TTSUrl, $"Bearer;{token}", requestString, upstreamLog);
            var response = JObject.Parse(Encoding.UTF8.GetString(responseRaw));
            LogHelper.LogInfo($"[{response["code"]}] {response["message"]}", upstreamLog);
            return response.Value<string>("data");
        }

        public static async Task<string> AutoSpeechRecognition(string appid, string token, string url, string data, string format, Action<string, LogLevel> upstreamLog = null) {
            var header = new List<KeyValuePair<string, string>>() {
                new("X-Api-App-Key", appid),
                new("X-Api-Access-Key", token),
                new("X-Api-Resource-Id", "volc.bigasr.auc_turbo"),
                new("X-Api-Request-Id", Guid.NewGuid().ToString()),
                new("X-Api-Sequence", "-1")
            };
            var request = new {
                audio = string.IsNullOrEmpty(url)
                    ? new {
                        url = "",
                        data,
                        format
                    }
                    : new {
                        url,
                        data = "",
                        format
                    },
                request = new {
                    model_name = "bigmodel",
                    enable_itn = true,
                    enable_punc = true
                }
            };
            var requestString = JsonConvert.SerializeObject(request);
            var responseRaw = await HttpHelper.PostJsonAsync(ASRUrl, "", requestString, header, upstreamLog);
            var response = JObject.Parse(Encoding.UTF8.GetString(responseRaw));
            return response["result"]?.Value<string>("text");
        }

        //object VoiceSynthRequest {
        //    object app;
        //        string appid;
        //        string token;
        //        string cluster;
        //    object user;
        //        string uid;
        //    object audio;
        //        string voice_type;
        //    object request;
        //        string reqid;
        //        string text;
        //        string operation;
        //}

        //object VoiceSynthResponse {
        //    string reqid;
        //    int code;
        //    string message;
        //    int sequence;
        //    string data;
        //    object addition;
        //        string duration;
        //        string frontend;
        //}

        //object ASRRequest {
        //    object audio;
        //        string url;       # *
        //        string data;
        //        string language;
        //        string format;    # * raw / wav / mp3 / ogg
        //        string codec;     # raw / opus
        //        int rate;         # 16000 (recommended)
        //    object request;
        //        string model_name;    # * Must be set to "bigmodel" for now.
        //        bool enable_itn;      # ITN will make the result more readable.
        //        bool enable_punc;
        //        bool enable_ddc;
        //        object corpus;
        //            string boosting_table_name;
        //            string correct_table_name;
        //            string context;
        //}

        //object ASRResponse {
        //    object audio_info;
        //        int duration;
        //    object result;
        //        string text;
        //}
        #endregion
    }
}
