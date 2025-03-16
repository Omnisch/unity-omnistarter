// author: Omnistudio
// version: 2025.03.16

using UnityEngine;

namespace Omnis.Utils
{
    public static class FloatHelper
    {
        #region Extensions using Mathf
        public static float Abs(this float value) => Mathf.Abs(value);
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
        /// <returns>1 - value</returns>
        public static float Inv(this float value) => 1f - value;
        #endregion
    }
}
