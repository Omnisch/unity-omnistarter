// author: Omnistudio
// version: 2025.06.11

#if UNITY_EDITOR
using Omnis.Utils;
using UnityEditor;
using UnityEngine;

namespace Omnis.Editor
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty minProp, GUIContent label)
        {
            var attr = (MinMaxAttribute)attribute;
            var maxProp = minProp.serializedObject.FindProperty($"max{attr.pairName}");

            if (maxProp == null || maxProp.propertyType != SerializedPropertyType.Float)
            {
                EditorGUI.HelpBox(position, $"Cannot find float {attr.pairName}", MessageType.Error);
                return;
            }

            EditorGUI.BeginProperty(position, label, minProp);
            EditorTweaker.MinMaxFloat(label.text.TrimStart("Min "), minProp, maxProp);
            EditorGUI.EndProperty();
        }
    }
}
#endif
