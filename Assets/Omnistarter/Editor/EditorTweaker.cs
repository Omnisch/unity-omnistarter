// author: Omnistudio
// version: 2025.06.09

using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Omnis.Editor
{
    /// <summary>
    /// Auxiliary functions of UnityEditor
    /// </summary>
    public class EditorTweaker : UnityEditor.Editor
    {
        #region Inspector
        /// <summary>
        /// Add one line of self script in Inspector.
        /// </summary>
        public static void Script(Object target, System.Type classType)
        {
            GUI.enabled = false;
            EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)System.Convert.ChangeType(target, classType)), classType, false);
            GUI.enabled = true;
        }

        /// <summary>
        /// Add one line of Header and prior space.
        /// </summary>
        public static void Header(string header)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
        }

        /// <summary>
        /// A shorthand for two fields that semantically are min and max.
        /// </summary>
        public static void MinMaxFloat(string label, SerializedProperty minProp, SerializedProperty maxProp)
        {
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(label, GUILayout.MaxWidth(100));
            EditorGUIUtility.labelWidth = 30;
            EditorGUILayout.PropertyField(minProp, new GUIContent("Min"));
            EditorGUILayout.PropertyField(maxProp, new GUIContent("Max"));

            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// A shorthand for two fields that semantically are min and max, using a EditorGUILayout.MinMaxSlider.
        /// </summary>
        public static void MinMaxSlider(string label, SerializedProperty minProp, SerializedProperty maxProp, float minLimit, float maxLimit)
        {
            EditorGUILayout.BeginHorizontal();

            float min = minProp.floatValue;
            float max = maxProp.floatValue;
            EditorGUILayout.LabelField(label, GUILayout.MinWidth(100));
            GUI.enabled = false;
            EditorGUILayout.PropertyField(minProp, GUIContent.none, GUILayout.MaxWidth(40));
            GUI.enabled = true;
            EditorGUILayout.MinMaxSlider(ref min, ref max, minLimit, maxLimit);
            GUI.enabled = false;
            EditorGUILayout.PropertyField(maxProp, GUIContent.none, GUILayout.MaxWidth(40));
            GUI.enabled = true;
            minProp.floatValue = min;
            maxProp.floatValue = max;

            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region Assets
        public static string ExtractAssetNameFromObject(Object obj)
            => ExtractAssetNameFromPath(AssetDatabase.GetAssetPath(obj));
        public static string ExtractAssetNameFromPath(string path)
            => string.Concat(path.Split('/').Last().Split('.')[0..^1]);
        public static string ExtractAssetMainNameFromObject(Object obj)
            => ExtractAssetMainNameFromPath(AssetDatabase.GetAssetPath(obj));
        public static string ExtractAssetMainNameFromPath(string path)
            => path.Split('/').Last().Split('.')[0];
        #endregion
    }
}
