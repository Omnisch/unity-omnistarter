// author: Omnistudio
// version: 2024.10.28

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// Auxiliary functions of UnityEditor
    /// </summary>
    public class EditorTweaker : Editor
    {
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
    }
}
#endif
