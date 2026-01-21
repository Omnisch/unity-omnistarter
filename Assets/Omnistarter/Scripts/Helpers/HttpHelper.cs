// author: Omnistudio
// version: 2026.01.21

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Omnis.Utils
{
    public static class HttpHelper
    {
        #region Make Request
        public static UnityWebRequest MakeGet(
            string url,
            string auth,
            List<KeyValuePair<string, string>> moreHeaders = null)
        {
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbGET)
            {
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Authorization", auth);

            if (moreHeaders != null)
                foreach (var header in moreHeaders)
                    request.SetRequestHeader(header.Key, header.Value);

            return request;
        }

        public static UnityWebRequest MakePostJsonByString(
            string url,
            string auth,
            string jsonBody = "{}",
            List<KeyValuePair<string, string>> moreHeaders = null)
        {
            jsonBody = JsonPruneHelperLight.RemoveEmptyStringFields(jsonBody);
            Debug.Log($"POST data: {jsonBody}");

            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody)),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Authorization", auth);
            request.SetRequestHeader("Content-Type", "application/json");
            
            if (moreHeaders != null)
                foreach (var header in moreHeaders)
                    request.SetRequestHeader(header.Key, header.Value);

            return request;
        }

        public static UnityWebRequest MakePostJsonFromStruct(
            string url,
            string auth,
            object body = null,
            List<KeyValuePair<string, string>> moreHeaders = null)
            => MakePostJsonByString(url, auth, JsonConvert.SerializeObject(body), moreHeaders);
        #endregion

        public static Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest request)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            request.SendWebRequest().completed += _ => tcs.SetResult(request);
            return tcs.Task;
        }

        public static byte[] GetResultData(UnityWebRequest request, Action<string, LogLevel> upstreamLog = null)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                LogHelper.LogInfo($"HTTP Success: {request.downloadHandler.text}", upstreamLog);
                return request.downloadHandler.data;
            }
            else
            {
                LogHelper.LogError($"HTTP Error: {request.responseCode} - {request.error}", upstreamLog);
                return null;
            }
        }

        #region Full Shorthands
        public async static Task<byte[]> GetAsync(string url, string auth, Action<string, LogLevel> upstreamLog = null) => await GetAsync(url, auth, null, upstreamLog);
        public async static Task<byte[]> GetAsync(
            string url,
            string auth,
            List<KeyValuePair<string, string>> moreHeaders,
            Action<string, LogLevel> upstreamLog = null)
        {
            UnityWebRequest request = MakeGet(url, auth, moreHeaders);
            LogHelper.LogInfo($"Waiting response from {url} ...", upstreamLog);

            await request.SendWebRequestAsync();
            return GetResultData(request, upstreamLog);
        }


        public async static Task<byte[]> PostJsonAsync(string url, string auth, object body, Action<string, LogLevel> upstreamLog = null) => await PostJsonAsync(url, auth, body, null, upstreamLog);
        public async static Task<byte[]> PostJsonAsync(
            string url,
            string auth,
            object body,
            List<KeyValuePair<string, string>> moreHeaders,
            Action<string, LogLevel> upstreamLog = null)
        {
            UnityWebRequest request;

            if (body.GetType() == typeof(string))
                request = MakePostJsonByString(url, auth, (string)body, moreHeaders);
            else
                request = MakePostJsonFromStruct(url, auth, body, moreHeaders);

            LogHelper.LogInfo($"Waiting response from {url} ...", upstreamLog);

            await request.SendWebRequestAsync();
            return GetResultData(request, upstreamLog);
        }
        #endregion
    }
}
