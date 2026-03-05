// author: Omnistudio
// version: 2026.03.05

using System.IO;
using UnityEngine;

namespace Omnis
{
    public static class AppDataLocation
    {
        public static string GetPortableDir(string folderName = "Saves") {
            var dataPath = Application.dataPath;
            var parent = Directory.GetParent(dataPath)?.FullName ?? dataPath;
            return Path.Combine(parent, folderName);
        }

        /// <summary>
        /// Returns a runtime directory as portable as possible.<br/>
        /// If the portable directory is not writable, returns persistentDataPath.
        /// </summary>
        public static string GetPortableDirWithFallback(string folderName = "Saves") {
            // 1) portable
            var portable = GetPortableDir(folderName);
            if (CanWriteToDirectory(portable))
                return portable;

            // 2) persistentDataPath
            var persistent = Path.Combine(Application.persistentDataPath, folderName);
            Directory.CreateDirectory(persistent);
            return persistent;
        }

        private static bool CanWriteToDirectory(string dir) {
            try {
                Directory.CreateDirectory(dir);
                var testFile = Path.Combine(dir, ".write_test");
                File.WriteAllText(testFile, "ok");
                File.Delete(testFile);
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
