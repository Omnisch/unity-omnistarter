// author: Omnistudio
// version: 2025.11.01

using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Omnis.Editor
{
    public class AssetsChangedTweaker : AssetPostprocessor
    {
        private const string PrefKey = "Omnis.UpdateHeaderDateEnabled";
        private const string MenuPath = "Tools/Omnis/Auto-Update Header Date";

        // Read user toggle (default off)
        private static bool UpdateDateEnabled => EditorPrefs.GetBool(PrefKey, false);


        // Keep menu checkmark in sync at editor load
        [InitializeOnLoadMethod]
        private static void InitMenu() {
            Menu.SetChecked(MenuPath, UpdateDateEnabled);
        }

        [MenuItem(MenuPath)]
        private static void ToggleUpdateHeaderDate() {
            bool v = !UpdateDateEnabled;
            EditorPrefs.SetBool(PrefKey, v);
            Menu.SetChecked(MenuPath, v);
            Debug.Log($"[AssetsChangedTweaker] Auto-update header date: {(v ? "ENABLED" : "DISABLED")}");
        }

        [MenuItem(MenuPath, true)]
        private static bool ToggleUpdateHeaderDateValidate() {
            Menu.SetChecked(MenuPath, UpdateDateEnabled);
            return true;
        }


        private static readonly Regex VersionLine = new(
            @"^//\s*version\s*:\s*(\d{4}\.\d{2}\.\d{2})",
            RegexOptions.Multiline | RegexOptions.Compiled);
        private static readonly Regex LockdateLine = new(
            @"^//\s*@lockdate",
            RegexOptions.Multiline | RegexOptions.Compiled);

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths) {
            if (!UpdateDateEnabled) return;

            string projectPath = Application.dataPath[..^"Assets".Length];
            string today = DateTime.Now.ToString("yyyy.MM.dd");

            foreach (string path in importedAssets) {
                if (!path.EndsWith(".cs", StringComparison.OrdinalIgnoreCase)) continue;

                string fullPath = projectPath + path;
                if (!File.Exists(fullPath)) continue;

                string content = File.ReadAllText(fullPath);

                // Per-file opt-out: put "// @lockdate" anywhere in the file to skip updates
                Match lck = LockdateLine.Match(content);
                if (lck.Success) continue;

                Match m = VersionLine.Match(content);
                if (!m.Success) continue;

                string current = m.Groups[1].Value;
                if (current == today) continue;

                string updated = VersionLine.Replace(content, $"// version: {today}", 1);
                if (updated == content) continue;

                File.WriteAllText(fullPath, updated);
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
        }
    }
}
