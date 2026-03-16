// author: Omnistudio
// version: 2026.03.16

using Omnis;
using Omnis.Audio;
using Omnis.Gizmos;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OmnisEditor
{
    public static class CreateMenuItem
    {
        #region Assets

        [MenuItem("Assets/Create/Omnis/Audio/Audio Cue from Clips", false, 241)]
        private static void CreateAudioCue() {
            var clips = Selection
                .GetFiltered<AudioClip>(SelectionMode.Assets)
                .Where(clip => clip != null)
                .ToArray();

            var asset = ScriptableObject.CreateInstance<AudioCue>();
            asset.clips = clips;
            
            var firstClipPath = AssetDatabase.GetAssetPath(clips[0]);
            var folder = System.IO.Path.GetDirectoryName(firstClipPath);
            var path = AssetDatabase.GenerateUniqueAssetPath(
                $"{folder}/New Audio Cue.asset");

            ProjectWindowUtil.CreateAsset(asset, path);
            // AssetDatabase.CreateAsset(asset, path);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
            //
            // EditorUtility.FocusProjectWindow();
            // Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/Omnis/Audio/Audio Cue from Clips", true)]
        private static bool ValidateCreateAudioCue()
            => Selection.GetFiltered<AudioClip>(SelectionMode.Assets).Length > 0;
        
        
        [MenuItem("Assets/Create/Omnis/Audio/Music Cue from Clips", false, 243)]
        private static void CreateMusicCue() {
            var clips = Selection
                .GetFiltered<AudioClip>(SelectionMode.Assets)
                .Where(clip => clip != null)
                .ToArray();

            var asset = ScriptableObject.CreateInstance<MusicCue>();
            asset.variants = clips.Select(clip => new MusicVariant { id = string.Empty, clip = clip, defaultVolume = 1 }).ToArray();
            
            var firstClipPath = AssetDatabase.GetAssetPath(clips[0]);
            var folder = System.IO.Path.GetDirectoryName(firstClipPath);
            var path = AssetDatabase.GenerateUniqueAssetPath(
                $"{folder}/New Music Cue.asset");

            ProjectWindowUtil.CreateAsset(asset, path);
            // AssetDatabase.CreateAsset(asset, path);
            // AssetDatabase.SaveAssets();
            // AssetDatabase.Refresh();
            //
            // EditorUtility.FocusProjectWindow();
            // Selection.activeObject = asset;
        }

        [MenuItem("Assets/Create/Omnis/Audio/Music Cue from Clips", true)]
        private static bool ValidateCreateMusicCue()
            => Selection.GetFiltered<AudioClip>(SelectionMode.Assets).Length > 0;
        
        
        [MenuItem("Assets/Create/Omnis/Class Editor", false, 244)]
        private static void CreateClassEditor() {
            string templatePath = "Assets/Omnistarter/Editor/ClassEditorTemplate.cs.txt";
            string selectedObjectName = EditorHelper.ExtractAssetMainNameFromObject(Selection.activeObject);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, $"{selectedObjectName}Editor.cs");
        }
        
        [MenuItem("Assets/Create/Omnis/Class Editor", true)]
        private static bool ValidateCreateClassEditor()
            => Selection.activeObject != null && AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(".cs");


        [MenuItem("Assets/Create/Omnis/Singleton", false, 245)]
        private static void CreateSingletonInstance() {
            string templatePath = "Assets/Omnistarter/Editor/SingletonTemplate.cs.txt";
            string selectedObjectName = EditorHelper.ExtractAssetMainNameFromObject(Selection.activeObject);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, $"{selectedObjectName}.singleton.cs");
        }
        
        [MenuItem("Assets/Create/Omnis/Singleton", true)]
        // Validate only when the selected asset is a csharp script.
        private static bool ValidateCreateSingletonInstance()
            => Selection.activeObject != null && AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(".cs");
        
        #endregion
        
        
        #region GameObjects
        
        [MenuItem("GameObject/Omnis/Gizmo Handler", false)]
        private static void CreateGizmoHandler(MenuCommand menuCommand) {
            GameObject go = new("Gizmo Handler");
            go.AddComponent<GizmoHandler>();
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Selection.activeObject = go;
        }
        
        [MenuItem("GameObject/Omnis/Input Router", false)]
        private static void CreateInputRouter(MenuCommand menuCommand) {
            GameObject go = new("Input Router");
            go.AddComponent<InputRouter>();
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Selection.activeObject = go;
        }
        
        #endregion
    }
}
