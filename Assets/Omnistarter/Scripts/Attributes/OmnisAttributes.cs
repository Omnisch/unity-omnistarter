// author: Omnistudio
// version: 2025.11.02

using System;
using UnityEngine;

namespace Omnis.Editor
{
    public sealed class InspectorReadOnlyAttribute : PropertyAttribute { }



    /// <summary>
    /// Put this on a serializable field (struct/class) to let ConditionalGroupDrawer render its children.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class ConditionalGroupAttribute : PropertyAttribute { }

    /// <summary>
    /// Show this field if the sibling property equals to one of the given values.<br/>
    /// AND when multiple attributes applyed.<br/>
    /// Supports: bool, enum, int, float, string (equals).
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class ShowIfAttribute : PropertyAttribute
    {
        public readonly string OtherProperty;
        public readonly object[] AnyEquals;  // enum literals allowed
        public ShowIfAttribute(string otherProperty, params object[] anyEquals) {
            OtherProperty = otherProperty;
            AnyEquals = anyEquals ?? Array.Empty<object>();
        }
    }

    /// <summary>
    /// Hide this field if the sibling property equals to one of the given values.<br/>
    /// AND when multiple attributes applyed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class HideIfAttribute : PropertyAttribute
    {
        public readonly string OtherProperty;
        public readonly object[] AnyEquals;
        public HideIfAttribute(string otherProperty, params object[] anyEquals) {
            OtherProperty = otherProperty;
            AnyEquals = anyEquals ?? Array.Empty<object>();
        }
    }

    /// <summary>
    /// Disable this field if the sibling property equals to one of the given values.<br/>
    /// OR when multiple attributes applyed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisableIfAttribute : PropertyAttribute
    {
        public readonly string OtherProperty;
        public readonly object[] AnyEquals;
        public DisableIfAttribute(string otherProperty, params object[] anyEquals) {
            OtherProperty = otherProperty;
            AnyEquals = anyEquals ?? Array.Empty<object>();
        }
    }
}
