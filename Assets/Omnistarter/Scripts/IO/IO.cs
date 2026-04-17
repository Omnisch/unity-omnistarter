// author: Omnistudio
// version: 2026.04.04

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
        public static bool SaveToFile(object data, string path, bool backupOne = false, bool useBinary = false) {
            return backupOne
                ? SaveWithBackup(data, path, useBinary)
                : SaveWithoutBackup(data, path, useBinary);
        }
        public static bool SaveToFile(object data, string dir, string fileName, bool backupOne = false, bool useBinary = false) {
            return backupOne
                ? SaveWithBackup(data, Path.Combine(dir, fileName), useBinary)
                : SaveWithoutBackup(data, Path.Combine(dir, fileName), useBinary);
        }
        private static bool SaveWithoutBackup(object data, string path, bool useBinary)
        {
            if (data is not byte[] bytes) {
                bytes = SerializationUtility.SerializeValue(data, useBinary ? DataFormat.Binary : DataFormat.JSON);
            }

            if (bytes == null || bytes.Length == 0) {
                Debug.LogError("Null or empty bytes. Saving aborted.");
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
        private static bool SaveWithBackup(object data, string path, bool useBinary) {
            string dir = Path.GetDirectoryName(path) ?? string.Empty;
            string name = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            string prevPath = Path.Combine(dir, $"{name}-prev{ext}");

            try {
                string tmpPath = path + ".tmp";
                SaveWithoutBackup(data, tmpPath, useBinary);

                if (File.Exists(path)) {
                    if (File.Exists(prevPath)) {
                        File.Delete(prevPath);
                    }

                    File.Move(path, prevPath);
                }

                File.Move(tmpPath, path);
                return true;
            }
            catch {
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



        /// <summary>
        /// Warning: Don't use this to load bytes, use File.ReadAllBytes instead.
        /// </summary>
        public static T LoadFromFile<T>(string path, bool useBinary = false)
        {
            byte[] bytes = File.ReadAllBytes(path);

            return SerializationUtility.DeserializeValue<T>(bytes, useBinary ? DataFormat.Binary : DataFormat.JSON);
        }
    }
}
