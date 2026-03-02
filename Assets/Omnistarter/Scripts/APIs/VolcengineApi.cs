// author: Omnistudio
// version: 2026.03.02

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
        public static readonly string ImageUrl = "https://ark.cn-beijing.volces.com/api/v3/images/generations";
        public static readonly string TTSUrl = "https://openspeech.bytedance.com/api/v1/tts";
        public static readonly string ASRUrl = "https://openspeech.bytedance.com/api/v3/auc/bigmodel/recognize/flash";


        #region Image
        /// <param name="request">You can use VolcengineApi.CombineRequest() to easily combine a request structure.</param>
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
                model = GetImageModelID(model),
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
        public static string GetImageModelID(ImageModel m) {
            return m switch {
                ImageModel.doubao_seedream_5_0 => "doubao-seedream-5-0-260128",
                ImageModel.doubao_seedream_4_5 => "doubao-seedream-4-5-251128",
                ImageModel.doubao_seedream_4_0 => "doubao-seedream-4-0-250828",
                _ => "doubao-seedream-4-0-250828"
            };
        }

        public class ImageResponseDataItem {
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
        #endregion


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
    }
}
