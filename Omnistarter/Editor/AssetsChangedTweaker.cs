// author: Omnistudio
// version: 2025.03.15

using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Omnis
{
    public class AssetsChangedTweaker : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            //EditorApplication.delayCall += () =>
            {
                foreach (string path in importedAssets)
                {
                    if (path.EndsWith(".cs"))
                    {
                        string fullPath = Application.dataPath[..^"Assets".Length] + path;

                        if (File.Exists(fullPath))
                        {
                            string content = File.ReadAllText(fullPath);

                            string toMatch = @"// version: (\d{4}\.\d{2}\.\d{2})";
                            Match m = Regex.Match(content, toMatch);
                            if (m.Success)
                            {
                                if (DateTime.Parse(m.Groups[1].Captures[0].Value) != DateTime.Now.Date)
                                {
                                    content = Regex.Replace(content, toMatch, "// version: " + DateTime.Now.ToString("yyyy.MM.dd"));
                                    File.WriteAllText(fullPath, content);
                                    AssetDatabase.Refresh();
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}
