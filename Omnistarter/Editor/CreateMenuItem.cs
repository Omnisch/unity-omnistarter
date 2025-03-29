// author: Omnistudio
// version: 2025.03.30

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
            string selectedObjectName = EditorTweaker.ExtractAssetMainNameFromObject(Selection.activeObject);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, $"{selectedObjectName}.singleton.cs");
        }

        [MenuItem("Assets/Create/Omnis/Singleton", true, 241)]
        // Validate only when the selected asset is a csharp script.
        private static bool ValidateCreateSingletonInstance()
        {
            return Selection.activeObject != null && AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(".cs");
        }
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
        #endregion
    }
}
