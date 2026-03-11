// author: Omnistudio
// version: 2026.03.11

using Omnis.Utils;
using OmnisEditor;
using System;
using UnityEngine;

namespace Omnis
{
    [Serializable]
    public class EasingSettings
    {
        [SerializeField] private bool useCustomCurve;
        [ShowIf("useCustomCurve", true)]
        [SerializeField] private AnimationCurve curve;
        [ShowIf("useCustomCurve", false)]
        [SerializeField] private Easing.EasingType type;


        public float Evaluate(float value) => useCustomCurve ? curve.Evaluate(value) : Easing.Select(type)(value);

        public EasingSettings(Easing.EasingType type) { this.type = type; }
    }
}
