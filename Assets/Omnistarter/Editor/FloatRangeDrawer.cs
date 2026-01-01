// author: Omnistudio
// version: 2026.01.02

using Omnis.Utils;
using UnityEditor;
using UnityEngine;

namespace Omnis.Editor
{
    [CustomPropertyDrawer(typeof(FloatRange))]
    public class FloatRangeDrawer : PropertyDrawer
    {
        private static readonly float vSpace = EditorGUIUtility.standardVerticalSpacing;
        private const float pad = 4f;
        private const float moreButtonWidth = 20f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            var min = property.FindPropertyRelative("min");
            var max = property.FindPropertyRelative("max");
            var limInf = property.FindPropertyRelative("limInf");
            var limSup = property.FindPropertyRelative("limSup");
            var allowZero = property.FindPropertyRelative("allowZeroWidth");

            // Draw min-max text
            Rect rFlatContent = EditorGUI.PrefixLabel(position, new GUIContent(label.text));

            float oldLabel = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 30f;

            float fieldWidth = Mathf.Max(0f, rFlatContent.width - 2 * pad - moreButtonWidth) * 0.5f;
            Rect rMin = new(rFlatContent.x, rFlatContent.y, fieldWidth, rFlatContent.height);
            Rect rMax = new(rMin.xMax + pad, rFlatContent.y, fieldWidth, rFlatContent.height);
            Rect rButton = new(rMax.xMax + pad, rFlatContent.y, moreButtonWidth, rFlatContent.height);

            EditorGUI.PropertyField(rMin, min, includeChildren: true);
            EditorGUI.PropertyField(rMax, max, includeChildren: true);
            // "More" button
            if (GUI.Button(rButton, EditorGUIUtility.TrTextContent("..."), EditorStyles.miniButton))
                property.isExpanded = !property.isExpanded;

            EditorGUIUtility.labelWidth = oldLabel;

            // Draw other fields
            if (property.isExpanded) {
                EditorGUI.indentLevel++;

                // Draw limit text
                Rect rLim = new(
                    position.x, position.y + EditorGUIUtility.singleLineHeight + vSpace,
                    position.width, EditorGUIUtility.singleLineHeight);
                Rect rLimContent = EditorGUI.PrefixLabel(rLim, new GUIContent("Limit"));

                EditorGUI.indentLevel--;
                EditorGUIUtility.labelWidth = 30f;

                Rect rLimInf = new(rLimContent.x, rLimContent.y, fieldWidth, rLimContent.height);
                Rect rLimSup = new(rLimInf.xMax + pad, rLimContent.y, fieldWidth, rLimContent.height);

                EditorGUI.PropertyField(rLimInf, limInf, new GUIContent("Inf"));
                EditorGUI.PropertyField(rLimSup, limSup, new GUIContent("Sup"));

                EditorGUIUtility.labelWidth = oldLabel;
                EditorGUI.indentLevel++;

                // Draw allow-zero text
                Rect rAllowZero = new(
                    position.x, position.y + 2 * (EditorGUIUtility.singleLineHeight + vSpace),
                    position.width, EditorGUIUtility.singleLineHeight);
                EditorGUI.PropertyField(rAllowZero, allowZero, true);

                EditorGUI.indentLevel--;
            }

            // Apply limits
            if (allowZero.boolValue) {
                limInf.floatValue = Mathf.Min(limInf.floatValue, limSup.floatValue);
                min.floatValue = Mathf.Clamp(min.floatValue, limInf.floatValue, max.floatValue);
                max.floatValue = Mathf.Clamp(max.floatValue, min.floatValue, limSup.floatValue);
            } else {
                limInf.floatValue = Mathf.Min(limInf.floatValue, limSup.floatValue - FloatHelper.EpsilonLoose);
                min.floatValue = Mathf.Clamp(min.floatValue, limInf.floatValue, max.floatValue - FloatHelper.EpsilonLoose);
                max.floatValue = Mathf.Clamp(max.floatValue, min.floatValue + FloatHelper.EpsilonLoose, limSup.floatValue);
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float lineH = EditorGUIUtility.singleLineHeight;
            return property.isExpanded ? 3 * lineH + 2 * vSpace : lineH;
        }
    }
}
