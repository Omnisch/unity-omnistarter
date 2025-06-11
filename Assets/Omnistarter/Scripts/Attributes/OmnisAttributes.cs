// author: Omnistudio
// version: 2025.06.11

using UnityEngine;

namespace Omnis.Editor
{
    public sealed class InspectorReadOnlyAttribute : PropertyAttribute { }
    /// <summary>Add the attribute to the min float and hide the max float in the inspector.<br/>
    /// E.g. [MinMax("Value")] float minValue; [HideInInspector] float maxValue;</summary>
    public sealed class MinMaxAttribute : PropertyAttribute
    {
        public readonly string pairName;
        public MinMaxAttribute(string pairName)
        {
            this.pairName = pairName;
        }
    }
}
