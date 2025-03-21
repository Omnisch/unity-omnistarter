// author: Omnistudio
// version: 2025.03.21

using UnityEngine;

namespace Omnis.Utils
{
    public static class FloatHelper
    {
        #region Extensions using Mathf
        public static float Abs(this float value) => Mathf.Abs(value);
        public static bool Approximately(this float value, float target) => Mathf.Approximately(value, target);
        public static float Ceil(this float value) => Mathf.Ceil(value);
        public static int CeilToInt(this float value) => Mathf.CeilToInt(value);
        public static float Clamp(this float value, float min, float max) => Mathf.Clamp(value, min, max);
        public static float Clamp01(this float value) => Mathf.Clamp01(value);
        public static float Floor(this float value) => Mathf.Floor(value);
        public static int FloorToInt(this float value) => Mathf.FloorToInt(value);
        public static float PingPong(this float value, float length) => Mathf.PingPong(value, length);
        public static float Pow(this float value, float p) => Mathf.Pow(value, p);
        public static float Repeat(this float value, float length) => Mathf.Repeat(value, length);
        public static float Round(this float value) => Mathf.Round(value);
        public static int RoundToInt(this float value) => Mathf.RoundToInt(value);
        public static float Sign(this float value) => Mathf.Sign(value);
        public static float Sqrt(this float value) => Mathf.Sqrt(value);
        #endregion


        #region Simple operations
        /// <summary>
        /// The loose version of Mathf.Approximately(), where Epsilon is 0.001.
        /// </summary>
        public static bool ApproxLoose(this float value, float target) => Mathf.Abs(target - value) < 0.001f;
        /// <returns>1 - value</returns>
        public static float Inv(this float value) => 1f - value;
        /// <summary>
        /// Basically same with Mathf.Repeat(),
        /// except when <i>value</i> % <i>length</i> = 0, it returns <i>length</i> instead of 0.
        /// </summary>
        public static float RepeatCeil(this float value, float length)
        {
            if (value == 0f) return 0f;
            float commonRepeat = Mathf.Repeat(value, length);
            return commonRepeat == 0f ? length : commonRepeat;
        }
        #endregion
    }
}
