// author: Omnistudio
// version: 2025.11.02

using UnityEditor;
using UnityEngine;

namespace Omnis.Editor
{
    public static class CreateMenuItem
    {
        #region Assets
        [MenuItem("Assets/Create/Omnis/Singleton", false, 241)]
        private static void CreateSingletonInstance()
        {
            string templatePath = "Assets/Omnistarter/Editor/SingletonTemplate.cs.txt";
            string selectedObjectName = EditorHelper.ExtractAssetMainNameFromObject(Selection.activeObject);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, $"{selectedObjectName}.singleton.cs");
        }

        [MenuItem("Assets/Create/Omnis/Singleton", true, 241)]
        // Validate only when the selected asset is a csharp script.
        private static bool ValidateCreateSingletonInstance()
            => Selection.activeObject != null && AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(".cs");



        [MenuItem("Assets/Create/Omnis/Class Editor", false, 241)]
        private static void CreateClassEditor()
        {
            string templatePath = "Assets/Omnistarter/Editor/ClassEditorTemplate.cs.txt";
            string selectedObjectName = EditorHelper.ExtractAssetMainNameFromObject(Selection.activeObject);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, $"{selectedObjectName}Editor.cs");
        }

        [MenuItem("Assets/Create/Omnis/Class Editor", true, 241)]
        private static bool ValidateCreateClassEditor()
            => Selection.activeObject != null && AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(".cs");
        #endregion

        #region GameObjects
        [MenuItem("GameObject/Omnis/Input Handler", false, 241)]
        private static void CreateInputHandler(MenuCommand menuCommand)
        {
            GameObject go = new("Input Handler");
            go.AddComponent<InputHandler>();
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Selection.activeObject = go;
        }

        [MenuItem("GameObject/Omnis/Gizmo Handler", false, 242)]
        private static void CreateGizmoHandler(MenuCommand menuCommand)
        {
            GameObject go = new("Gizmo Handler");
            go.AddComponent<Gizmos.GizmoHandler>();
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Selection.activeObject = go;
        }
        #endregion
    }
}
