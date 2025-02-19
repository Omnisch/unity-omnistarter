// author: Omnistudio
// version: 2025.02.19

using UnityEditor;

namespace Omnis
{
    public static class CreateMenuItem
    {
        [MenuItem("Assets/Create/Omnis/Singleton", false, 80)]
        private static void CreateSingletonInstance()
        {
            string templatePath = "Assets/Omnistarter/Editor/SingletonTemplate.cs.txt";
            string selectedObjectName = EditorTweaker.ExtractAssetMainNameFromObject(Selection.activeObject);
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(templatePath, $"{selectedObjectName}.singleton.cs");
        }

        [MenuItem("Assets/Create/Omnis/Singleton", true, 80)]
        // Validate only when the selected asset is a csharp script.
        private static bool ValidateCreateSingletonInstance()
        {
            return Selection.activeObject != null && AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(".cs");
        }
    }
}
