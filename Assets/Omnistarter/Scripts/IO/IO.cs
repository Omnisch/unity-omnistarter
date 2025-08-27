// author: Omnistudio
// version: 2025.08.27

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
        #region Multimodal
        /// <summary>
        /// If <i>dir</i> does not exist, create the directory.
        /// </summary>
        public static void EnsureDirectoryExists(string dir)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
                Debug.Log($"Created directory: {dir}");
            }
        }

        public static bool SaveToFile(this byte[] bytes, string path) => SaveBytesToFile(bytes, path);
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
            }

            return false;
        }
        #endregion

        #region Private Methods
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
        #endregion
    }
}
