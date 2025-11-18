// author: Omnistudio
// version: 2025.11.18

using Omnis.Utils;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Omnis.API
{
    public static class VolcengineApi
    {
        public static readonly string BaseUrl = "https://openspeech.bytedance.com/api/v1/tts";

        /// <returns>response.data</returns>
        public static async Task<string> VoiceSynthesis(string appid, string token, string prompt, string voice_type) {
            var request = new VoiceSynthRequest()
            {
                app = new()
                {
                    appid = appid,
                    token = token,
                    cluster = "volcano_tts",
                },
                user = new()
                {
                    uid = Application.buildGUID
                },
                audio = new()
                {
                    voice_type = voice_type,
                },
                request = new()
                {
                    reqid = Guid.NewGuid().ToString(),
                    text = prompt,
                    operation = "query"
                }
            };
            var requestString = JsonUtility.ToJson(request);
            var responseRaw = await HttpHelper.PostJsonAsync(BaseUrl, $"Bearer;{token}", requestString);
            var response = JsonUtility.FromJson<VoiceSynthResponse>(Encoding.UTF8.GetString(responseRaw));
            Debug.Log($"[{response.code}] {response.message}");
            return response.data;
        }

        #region Structs
        [Serializable]
        private class VoiceSynthRequest {
            public App app;
            public User user;
            public Audio audio;
            public Request request;

            [Serializable]
            public class App {
                public string appid;
                public string token;
                public string cluster;
            }
            [Serializable]
            public class User {
                public string uid;
            }
            [Serializable]
            public class Audio {
                public string voice_type;
            }
            [Serializable]
            public class Request {
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

            [Serializable]
            public class Addition {
                public string duration;
                public string frontend;
            }
        }
        #endregion
    }
}
