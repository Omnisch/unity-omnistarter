// author: Omnistudio
// version: 2025.11.03

using System;
using UnityEngine;

namespace Omnis
{
    [Serializable]
    public class FloatRange
    {
        [SerializeField] private float min;
        [SerializeField] private float max;
        [SerializeField] private float limInf = 0f;
        [SerializeField] public float limSup = 1f;
        public bool allowZeroWidth = true;


        public FloatRange(float min, float max) {
            Min = min; Max = max;
        }


        public float Min {
            get => min;
            set => min = Mathf.Clamp(value, limInf, max);
        }
        public float Max {
            get => max;
            set => max = Mathf.Clamp(value, min, limSup);
        }
        public void SetLimits(float inf, float sup) {
            limInf = inf; limSup = sup;
        }
    }
}
