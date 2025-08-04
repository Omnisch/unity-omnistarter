// author: Omnistudio
// version: 2025.08.04

using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Omnis.Utils
{
    public static class HttpHelper
    {
        public static UnityWebRequest SetRequestJsonByString(
            string url,
            string auth,
            string jsonBody = "{}",
            List<KeyValuePair<string, string>> moreHeaders = null)
        {
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody)),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Authorization", auth);
            request.SetRequestHeader("Content-Type", "application/json");
            
            if (moreHeaders != null)
            {
                foreach (var header in moreHeaders)
                    request.SetRequestHeader(header.Key, header.Value);
            }

            return request;
        }

        public static UnityWebRequest SetRequestJsonFromStruct(
            string url,
            string auth,
            object body = null,
            List<KeyValuePair<string, string>> moreHeaders = null)
        {
            string jsonBody = JsonUtility.ToJson(body);
            var request = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody)),
                downloadHandler = new DownloadHandlerBuffer()
            };
            request.SetRequestHeader("Authorization", auth);
            request.SetRequestHeader("Content-Type", "application/json");
            
            if (moreHeaders != null)
            {
                foreach (var header in moreHeaders)
                    request.SetRequestHeader(header.Key, header.Value);
            }

            return request;
        }

        public static Task<UnityWebRequest> SendWebRequestAsync(this UnityWebRequest request)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            request.SendWebRequest().completed += _ => tcs.SetResult(request);
            return tcs.Task;
        }

        public async static Task<byte[]> ApiJsonAsync(
            string url,
            string auth,
            object body = null,
            List<KeyValuePair<string, string>> moreHeaders = null)
        {
            UnityWebRequest request;

            if (body.GetType() == typeof(string))
                request = SetRequestJsonByString(url, auth, (string)body, moreHeaders);
            else
                request = SetRequestJsonFromStruct(url, auth, body, moreHeaders);

            Debug.Log($"Waiting response from {url} ...");

            await request.SendWebRequestAsync();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"HTTP Error: {request.responseCode} - {request.error}");
                return null;
            }
            else
            {
                Debug.Log($"HTTP Success: {request.downloadHandler.text}");
                return request.downloadHandler.data;
            }
        }
    }
}
