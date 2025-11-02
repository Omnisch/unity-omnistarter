// author: Omnistudio
// version: 2025.11.02
// drafted by GPT

using Omnis.Utils;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Omnis.Editor
{
    [CustomPropertyDrawer(typeof(ConditionalGroupAttribute))]
    public class ConditionalGroupDrawer : PropertyDrawer
    {
        private const float VSpace = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty(position, label, property);

            // Render children only, with default PropertyField behavior & proper heights.
            float y = position.y;
            var end = property.GetEndProperty();

            // Enter first child
            var child = property.Copy();
            bool hasChild = child.NextVisible(true) && !SerializedProperty.EqualContents(child, end);

            for (; hasChild; hasChild = child.NextVisible(false) && !SerializedProperty.EqualContents(child, end)) {
                // Skip script ref
                if (child.propertyPath.EndsWith(".m_Script"))
                    continue;

                if (IsVisible(child, property)) {
                    float h = EditorGUI.GetPropertyHeight(child, includeChildren: true);
                    var r = new Rect(position.x, y, position.width, h);

                    using (new EditorGUI.DisabledScope(IsDisabled(child, property))) {
                        EditorGUI.PropertyField(r, child, includeChildren: true);
                    }

                    y += h + VSpace;
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
            float height = 0f;
            var end = property.GetEndProperty();
            var child = property.Copy();

            bool hasChild = child.NextVisible(true) && !SerializedProperty.EqualContents(child, end);
            while (hasChild) {
                if (!child.propertyPath.EndsWith(".m_Script") && IsVisible(child, property)) {
                    height += EditorGUI.GetPropertyHeight(child, includeChildren: true) + VSpace;
                }
                hasChild = child.NextVisible(false) && !SerializedProperty.EqualContents(child, end);
            }
            // Remove last extra spacing
            if (height > 0f) height -= VSpace;
            return Mathf.Max(0f, height);
        }

        private static bool IsVisible(SerializedProperty fieldProp, SerializedProperty groupRoot) {
            var shows = GetAttributes<ShowIfAttribute>(fieldProp, groupRoot);
            var hides = GetAttributes<HideIfAttribute>(fieldProp, groupRoot);

            // AND all ShowIf; NAND all HideIf
            bool showOk = shows.All(a => CompareAgainst(groupRoot, a.OtherProperty, a.AnyEquals));
            bool hideOk = hides.All(a => !CompareAgainst(groupRoot, a.OtherProperty, a.AnyEquals));

            return showOk && hideOk;
        }

        private static bool IsDisabled(SerializedProperty fieldProp, SerializedProperty groupRoot) {
            var disables = GetAttributes<DisableIfAttribute>(fieldProp, groupRoot);
            // OR all DisableIf
            return disables.Any(a => CompareAgainst(groupRoot, a.OtherProperty, a.AnyEquals));
        }

        private static bool CompareAgainst(SerializedProperty groupRoot, string otherPropName, object[] anyEquals) {
            var other = groupRoot.FindPropertyRelative(otherPropName);
            if (other == null) return false;

            // If no values supplied: treat as "true"
            if (anyEquals == null || anyEquals.Length == 0) {
                if (other.propertyType == SerializedPropertyType.Boolean)
                    return other.boolValue;
                return true;
            }

            // Compare depending on type
            bool matched = false;
            switch (other.propertyType) {
                case SerializedPropertyType.Boolean:
                    matched = anyEquals.Any(v => THelper.ToBool(v) == other.boolValue);
                    break;
                case SerializedPropertyType.Enum:
                    // Match by name or index
                    string currentName = other.enumDisplayNames[other.enumValueIndex];
                    matched = anyEquals.Any(v => {
                        if (v == null) return false;
                        if (v.GetType().IsEnum) return string.Equals(v.ToString(), currentName, StringComparison.Ordinal);
                        if (v is string s) return string.Equals(s, currentName, StringComparison.Ordinal);
                        if (THelper.IsIntegerLike(v)) return Convert.ToInt32(v) == other.enumValueIndex;
                        return false;
                    });
                    break;
                case SerializedPropertyType.Integer:
                    matched = anyEquals.Any(v => THelper.IsIntegerLike(v) && Convert.ToInt32(v) == other.intValue);
                    break;
                case SerializedPropertyType.Float:
                    matched = anyEquals.Any(v => (v is float || v is double) && Mathf.Approximately((float)Convert.ToDouble(v), other.floatValue));
                    break;
                case SerializedPropertyType.String:
                    matched = anyEquals.Any(v => v is string s && s == other.stringValue);
                    break;
                default:
                    // Fallback: try string compare of displayed value
                    matched = anyEquals.Any(v => string.Equals(Convert.ToString(v), EditorHelper.GetDisplayString(other), StringComparison.Ordinal));
                    break;
            }

            return matched;
        }

        // Pull attributes from the declared FieldInfo of the child member of the group type.
        private static TAttr[] GetAttributes<TAttr>(SerializedProperty childProp, SerializedProperty groupRoot) where TAttr : Attribute {
            // childProp.name is the direct field name under the group
            // groupRoot's declared type:
            var groupField = GetDeclaredFieldInfo(groupRoot);
            var groupType = groupField != null ? GetFieldOrElementType(groupField.FieldType) : null;
            if (groupType == null) return Array.Empty<TAttr>();

            var fi = groupType.GetField(childProp.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return fi == null ? Array.Empty<TAttr>() : (TAttr[])fi.GetCustomAttributes(typeof(TAttr), inherit: true);
        }

        private static FieldInfo GetDeclaredFieldInfo(SerializedProperty prop) {
            // Accessing drawer.fieldInfo is valid: it's the FieldInfo for the *group* on the host component
            // but we are in a static method. We can't reach "fieldInfo" here, so we re-resolve:
            // Use reflection on the host object's type using the last segment of propertyPath.
            var target = prop.serializedObject.targetObject;
            if (target == null) return null;

            // Property path ends with ".<fieldName>" for the group itself; get final segment
            var path = prop.propertyPath;
            var lastDot = path.LastIndexOf('.');
            var fieldName = lastDot >= 0 ? path[(lastDot + 1)..] : path;

            var hostType = target.GetType();
            // Walk up to find the field (public or private)
            var fi = hostType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            return fi;
        }

        private static Type GetFieldOrElementType(Type t) {
            if (t.IsArray) return t.GetElementType();
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
                return t.GetGenericArguments()[0];
            return t;
        }
    }
}
