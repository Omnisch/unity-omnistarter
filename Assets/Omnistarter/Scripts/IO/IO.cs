// author: Omnistudio
// version: 2025.08.06

using AnotherFileBrowser.Windows;
using OdinSerializer;
using System.IO;
using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// Requiring packages: AnotherFileBrowser, OdinSerializer.
    /// </summary>
    public static class IO
    {
        #region File Browser
        /// <param name="extensionFilter">Format: "description (*.ext1, *.ext2) | *.ext1; *.ext2"</param>
        public static void OpenBrowserAndSaveFile<T>(T dataToSave, string extensionFilter)
        {
            BrowserProperties bp = new() { filter = extensionFilter, filterIndex = 0 };

            new FileBrowser().OpenFileBrowser(bp, path => SaveToFile(dataToSave, path));
        }
        /// <param name="extensionFilter">Format: "description (*.ext1, *.ext2) | *.ext1; *.ext2"</param>
        public static T OpenBrowserAndLoadFile<T>(string extensionFilter)
        {
            BrowserProperties bp = new() { filter = extensionFilter, filterIndex = 0 };

            T returnValue = default;
            new FileBrowser().OpenFileBrowser(bp, path => returnValue = LoadFromFile<T>(path));
            return returnValue;
        }
        /// <param name="extensionFilter">Format: "description (*.ext1, *.ext2) | *.ext1; *.ext2"</param>
        public static void OpenBrowserAndDo(string extensionFilter, System.Action<string> path)
        {
            BrowserProperties bp = new() { filter = extensionFilter, filterIndex = 0 };

            new FileBrowser().OpenFileBrowser(bp, path);
        }
        #endregion

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
        private static void SaveToFile<T>(T dataToSave, string path)
        {
            byte[] bytes = SerializationUtility.SerializeValue(dataToSave, DataFormat.JSON);
            File.WriteAllBytes(path, bytes);
        }
        private static T LoadFromFile<T>(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            return SerializationUtility.DeserializeValue<T>(bytes, DataFormat.JSON);
        }
        #endregion
    }
}
