// author: Omnistudio
// version: 2025.11.18

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
        public static async Task<string> UploadImage(string apiKey, byte[] imageData, string fileName) {
            string uploadUrl = $"{BaseUrl}/upload";
            string safeFileName = System.IO.Path.GetFileNameWithoutExtension(fileName) + ".jpg";
            safeFileName = safeFileName.Replace(" ", "_").Replace(",", "").Replace("'", "");

            var form = new WWWForm();
            form.AddBinaryData("file", imageData, safeFileName, "image/jpeg");

            using UnityWebRequest request = UnityWebRequest.Post(uploadUrl, form);
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");
            request.SetRequestHeader("Accept", "*/*");

            await request.SendWebRequestAsync();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"HTTP Error: {request.responseCode} - {request.error}");
                return null;
            }
            else
            {
                Debug.Log($"HTTP Success: {request.downloadHandler.text}");
                var data = request.downloadHandler.data;
                return data.GetJsonFieldByName<UploadResponse, string>("data.image_token");
            }
        }


        /// <returns>data.task_id</returns>
        public async static Task<string> CreateTextToImageTask(string apiKey, string prompt) {
            var requestData = new GenerationRequest
            {
                type = "generate_image",
                prompt = prompt
            };

            var data = await HttpHelper.PostJsonAsync($"{BaseUrl}/task", $"Bearer {apiKey}", requestData);
            return data.GetJsonFieldByName<GenerationResponse, string>("data.task_id");
        }

        /// <returns>data.task_id</returns>
        public async static Task<string> CreateImageToImageTask(string apiKey, string fileToken, string prompt, bool tPose) {
            var requestData = new GenerationRequest
            {
                type = "generate_image",
                prompt = prompt,
                file = new()
                {
                    type = "jpg",
                    file_token = fileToken
                },
                t_pose = tPose
            };

            var data = await HttpHelper.PostJsonAsync($"{BaseUrl}/task", $"Bearer {apiKey}", requestData);
            return data.GetJsonFieldByName<GenerationResponse, string>("data.task_id");
        }

        /// <returns>data.task_id</returns>
        public async static Task<string> CreateImageToModelTaskFromToken(string apiKey, string fileToken) {
            var requestData = new GenerationRequest
            {
                type = "image_to_model",
                file = new()
                {
                    type = "jpg",
                    file_token = fileToken
                },
                auto_size = true
            };

            var data = await HttpHelper.PostJsonAsync($"{BaseUrl}/task", $"Bearer {apiKey}", requestData);
            return data.GetJsonFieldByName<GenerationResponse, string>("data.task_id");
        }

        /// <returns>data.task_id</returns>
        public async static Task<string> CreateImageToModelTaskFromUrl(string apiKey, string url) {
            var requestData = new GenerationRequest
            {
                type = "image_to_model",
                file = new()
                {
                    type = "jpg",
                    url = url
                },
                auto_size = true
            };

            var data = await HttpHelper.PostJsonAsync($"{BaseUrl}/task", $"Bearer {apiKey}", requestData);
            return data.GetJsonFieldByName<GenerationResponse, string>("data.task_id");
        }


        /// <returns>data.task_id</returns>
        public async static Task<string> CreateAnimateRigTask(string apiKey, string taskId) {
            var requestData = new AnimationRequest
            {
                type = "animate_rig",
                original_model_task_id = taskId,
                out_format = ModelFormat
            };

            var data = await HttpHelper.PostJsonAsync($"{BaseUrl}/task", $"Bearer {apiKey}", requestData);
            return data.GetJsonFieldByName<GenerationResponse, string>("data.task_id");
        }


        /// <returns>data.output.pbr_model / data.output.model / data.output.generated_image</returns>
        public static async Task<string> CheckTaskStatus(string apiKey, string taskId, int interval = 10000, Action<float> percentageCallback = null) {
            Debug.Log("Start checking task status...");

            int maxRetries = 5;
            int currentRetry = 0;
            string outputUrl = string.Empty;

            while (currentRetry < maxRetries) {
                var downloadData = await HttpHelper.GetAsync($"{BaseUrl}/task/{taskId}", $"Bearer {apiKey}");

                if (downloadData != null) {
                    string downloadText = Encoding.UTF8.GetString(downloadData);
                    var statusResponse = JsonUtility.FromJson<TaskResponse>(downloadText);
                    if (statusResponse?.data != null) {
                        var data = statusResponse.data;
                        string currentStatus = data.status;

                        percentageCallback?.Invoke(data.progress / 100f);
                        Debug.Log($"Task Status: {currentStatus} | Progress: {data.progress}%");

                        if (currentStatus.ToLower() == "success") {
                            try {
                                outputUrl =
                                    !string.IsNullOrEmpty(data.output.pbr_model) ? data.output.pbr_model :
                                    !string.IsNullOrEmpty(data.output.model) ? data.output.model :
                                    data.output.generated_image;
                                Debug.Log($"Task completed, result URL: {outputUrl}");
                            }
                            catch {
                                Debug.LogError("Task completed but the result is empty.");
                            }
                            break;
                        } else if (currentStatus.ToLower() == "failed") {
                            Debug.LogError($"Task failed: {statusResponse.code}");
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

        #region Methods
        private static TField GetJsonFieldByName<TStruct, TField>(this byte[] data, string fieldName) {
            string json = Encoding.UTF8.GetString(data);
            TStruct rootObj;

            try {
                rootObj = JsonUtility.FromJson<TStruct>(json);
            }
            catch (Exception e) {
                Debug.LogError($"Data failed to fit TStruct: {e}");
                return default;
            }

            return rootObj.GetFieldByName<TStruct, TField>(fieldName);
        }
        #endregion

        #region Structs
        [Serializable]
        private class UploadResponse
        {
            public int code;
            public Data data;

            [Serializable]
            public class Data
            {
                public string image_token;
            }
        }

        [Serializable]
        private class GenerationRequest
        {
            public string type;
            public string prompt;
            public File file;
            public bool auto_size; // Image to Model
            public bool t_pose; // Advanced Generate Image

            [Serializable]
            public class File
            {
                public string type;
                public string file_token;
                public string url;
            }
        }

        [Serializable]
        private class AnimationRequest
        {
            public string type;
            public string original_model_task_id;
            public string out_format;
        }

        [Serializable]
        private class GenerationResponse
        {
            public int code;
            public Data data;

            [Serializable]
            public class Data
            {
                public string task_id;
            }
        }

        [Serializable]
        private class TaskResponse
        {
            public int code;
            public Data data;

            [Serializable]
            public class Data
            {
                public string task_id;
                public string type;
                public string status;
                public int progress;
                public Output output;

                [Serializable]
                public class Output
                {
                    public string model;
                    public string base_model;
                    public string pbr_model;
                    public string generated_image;
                }
            }
        }
        #endregion
    }
}
