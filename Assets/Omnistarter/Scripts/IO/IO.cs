// author: Omnistudio
// version: 2025.07.06

using AnotherFileBrowser.Windows;
using OdinSerializer;
using System.IO;

namespace Omnis
{
    /// <summary>
    /// Requiring packages: AnotherFileBrowser, OdinSerializer.
    /// </summary>
    public class IO
    {
        #region Interfaces
        /// <param name="extensionFilter">Format: "description (*.ext1, *.ext2) | *.ext1; *.ext2"</param>
        public static void OpenBrowserAndSaveFile<T>(T dataToSave, string extensionFilter)
        {
            BrowserProperties bp = new() { filter = extensionFilter, filterIndex = 0 };

            new FileBrowser().OpenFileBrowser(bp, path => SaveFile(dataToSave, path));
        }
        /// <param name="extensionFilter">Format: "description (*.ext1, *.ext2) | *.ext1; *.ext2"</param>
        public static T OpenBrowserAndLoadFile<T>(string extensionFilter)
        {
            BrowserProperties bp = new() { filter = extensionFilter, filterIndex = 0 };

            T returnValue = default;
            new FileBrowser().OpenFileBrowser(bp, path => returnValue = LoadFile<T>(path));
            return returnValue;
        }
        /// <param name="extensionFilter">Format: "description (*.ext1, *.ext2) | *.ext1; *.ext2"</param>
        public static void OpenBrowserAndDo(string extensionFilter, System.Action<string> filepath)
        {
            BrowserProperties bp = new() { filter = extensionFilter, filterIndex = 0 };

            new FileBrowser().OpenFileBrowser(bp, filepath);
        }
        #endregion

        #region Functions
        private static void SaveFile<T>(T dataToSave, string path)
        {
            byte[] bytes = SerializationUtility.SerializeValue(dataToSave, DataFormat.JSON);
            File.WriteAllBytes(path, bytes);
        }
        private static T LoadFile<T>(string path)
        {
            byte[] bytes = File.ReadAllBytes(path);
            return SerializationUtility.DeserializeValue<T>(bytes, DataFormat.JSON);
        }
        #endregion
    }
}
