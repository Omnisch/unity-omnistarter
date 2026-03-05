// author: Omnistudio
// version: 2026.03.03

using OdinSerializer;
using System.IO;
using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// Requiring packages: OdinSerializer
    /// </summary>
    public static class IO
    {
        public static bool SaveBytesToFile(byte[] bytes, string path)
        {
            if (bytes == null || bytes.Length == 0)
            {
                Debug.LogError("Null or empty byte data, saving aborted.");
                return false;
            }

            EnsureDirectoryExists(Path.GetDirectoryName(path));

            try
            {
                File.WriteAllBytes(path, bytes);
                Debug.Log($"Successfully wrote {bytes.Length} bytes to {path}");

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif

                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to write bytes: {e.Message}");
                return false;
            }
        }

        public static bool SaveWithBackup(byte[] bytes, string dir, string assetName, string lastAssetName) {
            EnsureDirectoryExists(dir);

            string assetPath = Path.Combine(dir, assetName);
            string lastPath = Path.Combine(dir, lastAssetName);

            try {
                string tmpPath = assetPath + ".tmp";
                File.WriteAllBytes(tmpPath, bytes);

                if (File.Exists(assetPath)) {
                    if (File.Exists(lastPath)) {
                        File.Delete(lastPath);
                    }

                    File.Move(assetPath, lastPath);
                }

                if (File.Exists(assetPath)) {
                    File.Delete(assetPath);
                }

                File.Move(tmpPath, assetPath);
                Debug.Log($"Successfully wrote {bytes.Length} bytes to {assetPath}");

#if UNITY_EDITOR
                UnityEditor.AssetDatabase.Refresh();
#endif
                return true;
            }
            catch (System.Exception e) {
                Debug.LogError($"Failed to write bytes: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// If <i>dir</i> does not exist, create the directory.
        /// </summary>
        private static void EnsureDirectoryExists(string dir) {
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
                Debug.Log($"Created directory: {dir}");
            }
        }



        public static void SaveToFile<T>(T dataToSave, string path)
        {
            byte[] bytes = SerializationUtility.SerializeValue(dataToSave, DataFormat.JSON);
            File.WriteAllBytes(path, bytes);
        }
        public static T LoadFromFile<T>(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            return SerializationUtility.DeserializeValue<T>(bytes, DataFormat.JSON);
        }
    }
}
