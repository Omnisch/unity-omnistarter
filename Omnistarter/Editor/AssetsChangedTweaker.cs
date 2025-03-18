// author: Omnistudio
// version: 2025.03.17

using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Omnis.Editor
{
    public class AssetsChangedTweaker : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            foreach (string path in importedAssets)
            {
                if (path.EndsWith(".cs"))
                {
                    string fullPath = Application.dataPath[..^"Assets".Length] + path;

                    if (File.Exists(fullPath))
                    {
                        string content = File.ReadAllText(fullPath);

                        string pattern = @"\d{4}\.\d{2}\.\d{2}";
                        Regex rgx = new(pattern);
                        Match m = rgx.Match(content);
                        if (m.Success)
                        {
                            if (DateTime.Parse(m.Groups[0].Value) != DateTime.Now.Date)
                            {
                                content = rgx.Replace(content, DateTime.Now.ToString("yyyy.MM.dd"), 1);
                                File.WriteAllText(fullPath, content);
                                AssetDatabase.Refresh();
                            }
                        }
                    }
                }
            }
        }
    }
}
