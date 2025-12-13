// author: Omnistudio
// version: 2025.12.13

using Omnis.Utils;
using System;
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
        public static async Task<string> VoiceSynthesis(string appid, string token, string prompt, string voice_type) {
            var request = new VoiceSynthRequest() {
                app = new() {
                    appid = appid,
                    token = token,
                    cluster = "volcano_tts",
                },
                user = new() {
                    uid = Application.buildGUID
                },
                audio = new() {
                    voice_type = voice_type,
                },
                request = new() {
                    reqid = Guid.NewGuid().ToString(),
                    text = prompt,
                    operation = "query"
                }
            };
            var requestString = JsonUtility.ToJson(request);
            var responseRaw = await HttpHelper.PostJsonAsync(TTSUrl, $"Bearer;{token}", requestString);
            var response = JsonUtility.FromJson<VoiceSynthResponse>(Encoding.UTF8.GetString(responseRaw));
            Debug.Log($"[{response.code}] {response.message}");
            return response.data;
        }

        public static async Task<string> AutoSpeechRecognition(string appid, string token, string url, string data, string format) {
            var header = new System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, string>>() {
                new("X-Api-App-Key", appid),
                new("X-Api-Access-Key", token),
                new("X-Api-Resource-Id", "volc.bigasr.auc_turbo"),
                new("X-Api-Request-Id", Guid.NewGuid().ToString()),
                new("X-Api-Sequence", "-1")
            };
            ASRRequest request;
            if (url != null) {
                request = new ASRRequest() {
                    audio = new() {
                        url = url,
                        format = format
                    },
                    request = new() {
                        enable_punc = true
                    }
                };
            } else {
                request = new ASRRequest() {
                    audio = new() {
                        data = data,
                        format = format
                    },
                    request = new() {
                        enable_punc = true
                    }
                };
            }
            var requestString = JsonUtility.ToJson(request);
            var responseRaw = await HttpHelper.PostJsonAsync(ASRUrl, "", requestString, header);
            var response = JsonUtility.FromJson<ASRResponse>(Encoding.UTF8.GetString(responseRaw));
            return response.result.text;
        }

        #region Structs
        [Serializable]
        private class VoiceSynthRequest {
            public App app;
            public User user;
            public Audio audio;
            public Request request;

            [Serializable] public class App {
                public string appid;
                public string token;
                public string cluster;
            }
            [Serializable] public class User {
                public string uid;
            }
            [Serializable] public class Audio {
                public string voice_type;
            }
            [Serializable] public class Request {
                public string reqid;
                public string text;
                public string operation;
            }
        }

        [Serializable]
        public class VoiceSynthResponse {
            public string reqid;
            public int code;
            public string message;
            public int sequence;
            public string data;
            public Addition addition;

            [Serializable] public class Addition {
                public string duration;
                public string frontend;
            }
        }

        /// <summary>
        /// These are mandatory:<br/>
        /// audio.url or audio.data<br/>
        /// audio.format<br/>
        /// request.model_name
        /// </summary>
        [Serializable]
        public class ASRRequest {
            public Audio audio;
            public Request request;

            [Serializable] public class Audio {
                public string url;
                public string data;
                public string language;
                /// <summary>
                /// raw / wav / mp3 / ogg
                /// </summary>
                public string format;
                /// <summary>
                /// raw / opus
                /// </summary>
                public string codec = "raw";
                /// <summary>
                /// 16000 by default.
                /// </summary>
                public int rate = 16000;
            }
            [Serializable] public class Request {
                /// <summary>
                /// Must be set to "bigmodel" for now.
                /// </summary>
                public string model_name = "bigmodel";
                /// <summary>
                /// ITN will make the result more readable.
                /// </summary>
                public bool enable_itn = true;
                public bool enable_punc = false;
                public bool enable_ddc = false;
                public Corpus corpus;

                [Serializable] public class Corpus {
                    public string boosting_table_name;
                    public string correct_table_name;
                    public string context;
                }
            }
        }

        [Serializable]
        public class ASRResponse {
            public Audio_info audio_info;
            public Result result;

            [Serializable] public class Audio_info {
                public int duration;
            }
            [Serializable] public class Result {
                public string text;
            }
        }
        #endregion
    }
}
