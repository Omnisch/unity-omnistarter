// author: Omnistudio
// version: 2025.08.06

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
        {
            string jsonBody = JsonUtility.ToJson(body);
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
        #endregion

        public static Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest request)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            request.SendWebRequest().completed += _ => tcs.SetResult(request);
            return tcs.Task;
        }

        public static byte[] GetResultData(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"HTTP Success: {request.downloadHandler.text}");
                return request.downloadHandler.data;
            }
            else
            {
                Debug.LogError($"HTTP Error: {request.responseCode} - {request.error}");
                return null;
            }
        }

        #region Full Shorthands
        public async static Task<byte[]> GetAsync(string url, string auth) => await GetAsync(url, auth, null);
        public async static Task<byte[]> GetAsync(
            string url,
            string auth,
            List<KeyValuePair<string, string>> moreHeaders)
        {
            UnityWebRequest request = MakeGet(url, auth, moreHeaders);
            Debug.Log($"Waiting response from {url} ...");

            await request.SendWebRequestAsync();
            return GetResultData(request);
        }


        public async static Task<byte[]> PostJsonAsync(string url, string auth, object body) => await PostJsonAsync(url, auth, body, null);
        public async static Task<byte[]> PostJsonAsync(
            string url,
            string auth,
            object body,
            List<KeyValuePair<string, string>> moreHeaders)
        {
            UnityWebRequest request;

            if (body.GetType() == typeof(string))
                request = MakePostJsonByString(url, auth, (string)body, moreHeaders);
            else
                request = MakePostJsonFromStruct(url, auth, body, moreHeaders);

            Debug.Log($"Waiting response from {url} ...");

            await request.SendWebRequestAsync();
            return GetResultData(request);
        }
        #endregion
    }
}
