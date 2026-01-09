// author: Omnistudio
// version: 2026.01.10

using Newtonsoft.Json.Linq;
using Omnis.Utils;
using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Omnis.API
{
    public static class Tripo3DApi
    {
        private static readonly string BaseUrl = "https://api.tripo3d.ai/v2/openapi";
        public static readonly string ModelFormat = "glb"; // (glb, fbx)


        /// <returns>data.image_token</returns>
        public static async Task<string> UploadImage(string api_key, byte[] imageData, string fileName) {
            string uploadUrl = $"{BaseUrl}/upload";
            string safeFileName = System.IO.Path.GetFileNameWithoutExtension(fileName) + ".jpg";
            safeFileName = safeFileName.Replace(" ", "_").Replace(",", "").Replace("'", "");

            var form = new WWWForm();
            form.AddBinaryData("file", imageData, safeFileName, "image/jpeg");

            using UnityWebRequest request = UnityWebRequest.Post(uploadUrl, form);
            request.SetRequestHeader("Authorization", $"Bearer {api_key}");
            request.SetRequestHeader("Accept", "*/*");

            await request.SendWebRequestAsync();

            if (request.result != UnityWebRequest.Result.Success) {
                Debug.LogError($"HTTP Error: {request.responseCode} - {request.error}");
                return null;
            } else {
                Debug.Log($"HTTP Success: {request.downloadHandler.text}");
                var dataRaw = request.downloadHandler.data;
                var data = JObject.Parse(Encoding.UTF8.GetString(dataRaw));
                return data.SelectToken("data.image_token").Value<string>();
            }
        }


        /// <returns>data.task_id</returns>
        public async static Task<string> CreateTextToImageTask(string api_key, string prompt) {
            var requestData = new {
                type = "generate_image",
                prompt
            };

            var dataRaw = await HttpHelper.PostJsonAsync($"{BaseUrl}/task", $"Bearer {api_key}", requestData);
            var data = JObject.Parse(Encoding.UTF8.GetString(dataRaw));
            return data.SelectToken("data.task_id").Value<string>();
        }

        /// <returns>data.task_id</returns>
        public async static Task<string> CreateImageToImageTask(string api_key, string file_token, string prompt, bool t_pose) {
            var requestData = new {
                type = "generate_image",
                prompt,
                file = new {
                    type = "jpg",
                    file_token
                },
                t_pose
            };

            var dataRaw = await HttpHelper.PostJsonAsync($"{BaseUrl}/task", $"Bearer {api_key}", requestData);
            var data = JObject.Parse(Encoding.UTF8.GetString(dataRaw));
            return data.SelectToken("data.task_id").Value<string>();
        }

        /// <returns>data.task_id</returns>
        public async static Task<string> CreateImageToModelTaskFromToken(string api_key, string file_token) {
            var requestData = new {
                type = "image_to_model",
                file = new {
                    type = "jpg",
                    file_token
                },
                auto_size = true
            };

            var dataRaw = await HttpHelper.PostJsonAsync($"{BaseUrl}/task", $"Bearer {api_key}", requestData);
            var data = JObject.Parse(Encoding.UTF8.GetString(dataRaw));
            return data.SelectToken("data.task_id").Value<string>();
        }

        /// <returns>data.task_id</returns>
        public async static Task<string> CreateImageToModelTaskFromUrl(string api_key, string url) {
            var requestData = new {
                type = "image_to_model",
                file = new {
                    type = "jpg",
                    url
                },
                auto_size = true
            };

            var dataRaw = await HttpHelper.PostJsonAsync($"{BaseUrl}/task", $"Bearer {api_key}", requestData);
            var data = JObject.Parse(Encoding.UTF8.GetString(dataRaw));
            return data.SelectToken("data.task_id").Value<string>();
        }


        /// <returns>data.task_id</returns>
        public async static Task<string> CreateAnimateRigTask(string api_key, string task_id) {
            var requestData = new {
                type = "animate_rig",
                original_model_task_id = task_id,
                out_format = ModelFormat
            };

            var dataRaw = await HttpHelper.PostJsonAsync($"{BaseUrl}/task", $"Bearer {api_key}", requestData);
            var data = JObject.Parse(Encoding.UTF8.GetString(dataRaw));
            return data.SelectToken("data.task_id").Value<string>();
        }


        /// <returns>data.output.pbr_model / data.output.model / data.output.generated_image</returns>
        public static async Task<string> CheckTaskStatus(string api_key, string task_id, int interval = 10000, Action<float> percentageCallback = null) {
            Debug.Log("Start checking task status...");

            int maxRetries = 5;
            int currentRetry = 0;
            string outputUrl = string.Empty;

            while (currentRetry < maxRetries) {
                var downloadData = await HttpHelper.GetAsync($"{BaseUrl}/task/{task_id}", $"Bearer {api_key}");

                if (downloadData != null) {
                    string downloadText = Encoding.UTF8.GetString(downloadData);
                    var statusResponse = JObject.Parse(downloadText);
                    if (statusResponse?["data"] != null) {
                        var data = statusResponse["data"];
                        var currentStatus = data?.Value<string>("status");
                        var progress = data?.Value<float>("progress");

                        percentageCallback?.Invoke(progress ?? 0f / 100f);
                        Debug.Log($"Task status: {currentStatus} | progress: {progress}%");

                        if (currentStatus.ToLower() == "success") {
                            try {
                                outputUrl =
                                    data?.SelectToken("output.pbr_model") != null ? data.SelectToken("output.pbr_model").Value<string>() :
                                    data?.SelectToken("output.model") != null ? data.SelectToken("output.model").Value<string>() :
                                    data?.SelectToken("output.generated_image").Value<string>();
                                Debug.Log($"Task completed, result URL: {outputUrl}");
                            }
                            catch {
                                Debug.LogError("Task completed but the result is empty.");
                            }
                            break;
                        } else if (currentStatus.ToLower() == "failed") {
                            Debug.LogError($"Task failed: {statusResponse?.Value<string>("code")}");
                            break;
                        }
                    } else {
                        if (++currentRetry >= maxRetries) {
                            Debug.LogError($"Failed to parse data, Retries have reached the limit {maxRetries}, the request is aborted.");
                        } else {
                            Debug.LogWarning($"Failed to parse data, {currentRetry}/{maxRetries} retries...");
                        }
                    }
                } else {
                    if (++currentRetry >= maxRetries) {
                        Debug.LogError($"Failed to request status. Retries have reached the limit {maxRetries}, the request is aborted.");
                    } else {
                        Debug.LogWarning($"Failed to request status, {currentRetry}/{maxRetries} retries...");
                    }
                }

                await Task.Delay(interval);
            }

            return outputUrl;
        }


        public static async Task<byte[]> DownloadAsset(string url, string apiKey) {
            Debug.Log($"Start downloading... URL: {url}");

            return await HttpHelper.GetAsync(url, $"Bearer {apiKey}");
        }


        #region Structs
        //object UploadResponse {
        //    int code;
        //    object data;
        //        string image_token;
        //}

        //object GenerationRequest {
        //    string type;
        //    string prompt;
        //    object file;
        //        string type;
        //        string file_token;
        //        string url;
        //    bool auto_size;       # Image to Model
        //    bool t_pose;          # Advanced Generate Image
        //}

        //object AnimationRequest {
        //    string type;
        //    string original_model_task_id;
        //    string out_format;
        //}

        //object GenerationResponse {
        //    int code;
        //    object data;
        //        string task_id;
        //}

        //object TaskResponse {
        //    int code;
        //    object data;
        //        string task_id;
        //        string type;
        //        string status;
        //        int progress;
        //        object output;
        //            string model;
        //            string base_model;
        //            string pbr_model;
        //            string generated_image;
        //}
        #endregion
    }
}
