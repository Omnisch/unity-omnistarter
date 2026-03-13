// author: Omnistudio
// version: 2026.03.13

using System;
using UnityEngine;

namespace OmnisEditor
{
    public sealed class InspectorReadOnlyAttribute : PropertyAttribute { }



    /// <summary>
    /// Put this on a serializable field (struct/class) to let ConditionalGroupDrawer render its children.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ConditionalGroupAttribute : PropertyAttribute { }

    /// <summary>
    /// Show this field if the sibling property equals to one of the given values.<br/>
    /// AND when multiple attributes applyed.<br/>
    /// Supports: bool, enum, int, float, string (equals).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly string otherProperty;
        public readonly object[] anyEquals;  // enum literals allowed
        public ShowIfAttribute(string otherProperty, params object[] anyEquals) {
            this.otherProperty = otherProperty;
            this.anyEquals = anyEquals ?? Array.Empty<object>();
        }
    }

    /// <summary>
    /// Hide this field if the sibling property equals to one of the given values.<br/>
    /// AND when multiple attributes applied.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HideIfAttribute : PropertyAttribute
    {
        public readonly string otherProperty;
        public readonly object[] anyEquals;
        public HideIfAttribute(string otherProperty, params object[] anyEquals) {
            this.otherProperty = otherProperty;
            this.anyEquals = anyEquals ?? Array.Empty<object>();
        }
    }

    /// <summary>
    /// Disable this field if the sibling property equals to one of the given values.<br/>
    /// OR when multiple attributes applied.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisableIfAttribute : PropertyAttribute
    {
        public readonly string otherProperty;
        public readonly object[] anyEquals;
        public DisableIfAttribute(string otherProperty, params object[] anyEquals) {
            this.otherProperty = otherProperty;
            this.anyEquals = anyEquals ?? Array.Empty<object>();
        }
    }
}
