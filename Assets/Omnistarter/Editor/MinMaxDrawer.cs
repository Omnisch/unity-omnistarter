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
        private const float gap = 4f;

        public override void OnGUI(Rect position, SerializedProperty minProp, GUIContent label)
        {
            var attr = (MinMaxAttribute)attribute;
            var maxProp = minProp.serializedObject.FindProperty($"max{attr.pairName}");

            if (maxProp == null || maxProp.propertyType != SerializedPropertyType.Float)
            {
                EditorGUI.HelpBox(position, $"Cannot find float {attr.pairName}", MessageType.Error);
                return;
            }

            Rect contentRect = EditorGUI.PrefixLabel(position, new GUIContent(label.text.TrimStart("Min ")));

            float fieldWidth = (contentRect.width - gap) * 0.5f;
            Rect minRect = new(contentRect.x, contentRect.y, fieldWidth, contentRect.height);
            Rect maxRect = new(minRect.xMax + gap, contentRect.y, fieldWidth, contentRect.height);

            float oldLabel = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 30f;

            EditorGUI.BeginProperty(minRect, GUIContent.none, minProp);
            EditorGUI.PropertyField(minRect, minProp, new GUIContent("Min"));
            EditorGUI.EndProperty();

            EditorGUI.BeginProperty(maxRect, GUIContent.none, maxProp);
            EditorGUI.PropertyField(maxRect, maxProp, new GUIContent("Max"));
            EditorGUI.EndProperty();

            EditorGUIUtility.labelWidth = oldLabel;
        }
    }
}
#endif
