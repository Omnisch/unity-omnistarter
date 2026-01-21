// author: Omnistudio
// version: 2026.01.21

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
        public static readonly string TTSUrl = "https://openspeech.bytedance.com/api/v1/tts";
        public static readonly string ASRUrl = "https://openspeech.bytedance.com/api/v3/auc/bigmodel/recognize/flash";

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

        #region Structs
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
