// author: Omnistudio
// version: 2026.01.21

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
        public static async Task<string> UploadImage(string api_key, byte[] imageData, string fileName, Action<string, LogLevel> upstreamLog = null) {
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
                LogHelper.LogError($"HTTP Error: {request.responseCode} - {request.error}", upstreamLog);
                return null;
            } else {
                LogHelper.LogInfo($"HTTP Success: {request.downloadHandler.text}", upstreamLog);
                var dataRaw = request.downloadHandler.data;
                var data = JObject.Parse(Encoding.UTF8.GetString(dataRaw));
                return data.SelectToken("data.image_token").Value<string>();
            }
        }


        /// <returns>data.task_id</returns>
        public async static Task<string> CreateTextToImageTask(string api_key, string prompt, Action<string, LogLevel> upstreamLog = null) {
            var requestData = new {
                type = "generate_image",
                prompt
            };

            var dataRaw = await HttpHelper.PostJsonAsync($"{BaseUrl}/task", $"Bearer {api_key}", requestData);
            var data = JObject.Parse(Encoding.UTF8.GetString(dataRaw));
            return data.SelectToken("data.task_id").Value<string>();
        }

        /// <returns>data.task_id</returns>
        public async static Task<string> CreateImageToImageTask(string api_key, string file_token, string prompt, bool t_pose, Action<string, LogLevel> upstreamLog = null) {
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
        public async static Task<string> CreateImageToModelTaskFromToken(string api_key, string file_token, Action<string, LogLevel> upstreamLog = null) {
            var requestData = new {
                type = "image_to_model",
                model_version = "v3.0-20250812",
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
        public async static Task<string> CreateImageToModelTaskFromUrl(string api_key, string url, Action<string, LogLevel> upstreamLog = null) {
            var requestData = new {
                type = "image_to_model",
                model_version = "v3.0-20250812",
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
        public async static Task<string> CreateAnimateRigTask(string api_key, string task_id, Action<string, LogLevel> upstreamLog = null) {
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
        public static async Task<string> CheckTaskStatus(string api_key, string task_id, int interval = 10000, Action<float> percentageCallback = null, Action<string, LogLevel> upstreamLog = null) {
            LogHelper.LogInfo("Start checking task status...", upstreamLog);

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
                        LogHelper.LogInfo($"Task status: {currentStatus} | progress: {progress}%", upstreamLog);

                        if (currentStatus.ToLower() == "success") {
                            try {
                                outputUrl =
                                    data?.SelectToken("output.pbr_model") != null ? data.SelectToken("output.pbr_model").Value<string>() :
                                    data?.SelectToken("output.model") != null ? data.SelectToken("output.model").Value<string>() :
                                    data?.SelectToken("output.generated_image").Value<string>();
                                LogHelper.LogInfo($"Task completed, result URL: {outputUrl}", upstreamLog);
                            }
                            catch {
                                LogHelper.LogError("Task completed but the result is empty.", upstreamLog);
                            }
                            break;
                        } else if (currentStatus.ToLower() == "failed") {
                            LogHelper.LogError($"Task failed: {statusResponse?.Value<string>("code")}", upstreamLog);
                            break;
                        }
                    } else {
                        if (++currentRetry >= maxRetries) {
                            LogHelper.LogError($"Failed to parse data, Retries have reached the limit {maxRetries}, the request is aborted.", upstreamLog);
                        } else {
                            LogHelper.LogWarning($"Failed to parse data, {currentRetry}/{maxRetries} retries...", upstreamLog);
                        }
                    }
                } else {
                    if (++currentRetry >= maxRetries) {
                        LogHelper.LogError($"Failed to request status. Retries have reached the limit {maxRetries}, the request is aborted.", upstreamLog);
                    } else {
                        LogHelper.LogWarning($"Failed to request status, {currentRetry}/{maxRetries} retries...", upstreamLog);
                    }
                }

                await Task.Delay(interval);
            }

            return outputUrl;
        }


        public static async Task<byte[]> DownloadAsset(string api_key, string url, Action<string, LogLevel> upstreamLog = null) {
            LogHelper.LogInfo($"Start downloading... URL: {url}", upstreamLog);

            return await HttpHelper.GetAsync(url, $"Bearer {api_key}");
        }


        #region Structs
        //object UploadResponse {
        //    int code;
        //    object data;
        //        string image_token;
        //}

        //object GenerationRequest {
        //    string type;
        //    string model_version; # v3.0-20250812 up to date
        //    string prompt;        # only available in Advanced Generate Image
        //    object file;
        //        string type;
        //        string file_token;
        //        string url;
        //    bool auto_size;       # only available in Image to Model
        //    bool t_pose;          # only available in Advanced Generate Image
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
