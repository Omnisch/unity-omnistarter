// author: Omnistudio
// version: 2026.02.01

using System;
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
        /// <summary>0.0001</summary>
        public static readonly float EpsilonLoose = 0.0001f;
        /// <summary>
        /// The loose version of Mathf.Approximately(), where Epsilon is 0.0001.
        /// </summary>
        public static bool ApproxLoose(this float value, float target) => Mathf.Abs(target - value) < EpsilonLoose;
        /// <returns>1 - value</returns>
        public static float Inv(this float value) => 1f - value;
        /// <summary>
        /// It's basically same with Mathf.Repeat(),
        /// except when <i>value</i> % <i>length</i> = 0, it returns <i>length</i> instead of 0.
        /// </summary>
        public static float RepeatCeil(this float value, float length)
        {
            if (value == 0f) return 0f;
            float commonRepeat = Mathf.Repeat(value, length);
            return commonRepeat == 0f ? length : commonRepeat;
        }
        /// <summary>
        /// A shorthand to System.Math.Round(value, digits).
        /// </summary>
        public static float Round(this float value, int digits) => (float)Math.Round(value, digits);
        /// <summary>
        /// If the value is greater than or equal to <i>par</i> it returns 1, else returns 0.
        /// </summary>
        public static float Step01(this float value, float par) => value >= par ? 1f : 0f;
        #endregion

        #region Float to Vector2 or Vector3
        /// <returns>(value, value)</returns>
        public static Vector2 StickV2(this float value) => new(value, value);
        /// <returns>(value, value, value)</returns>
        public static Vector3 StickV3(this float value) => new(value, value, value);
        /// <returns>(value, 0)</returns>
        public static Vector2 no(this float value) => new(value, 0f);
        /// <returns>(0, value)</returns>
        public static Vector2 on(this float value) => new(0f, value);
        /// <returns>(value, 0, 0)</returns>
        public static Vector3 noo(this float value) => new(value, 0f, 0f);
        /// <returns>(0, value, 0)</returns>
        public static Vector3 ono(this float value) => new(0f, value, 0f);
        /// <returns>(0, 0, value)</returns>
        public static Vector3 oon(this float value) => new(0f, 0f, value);
        /// <returns>(value, value, 0)</returns>
        public static Vector3 nno(this float value) => new(value, value, 0f);
        /// <returns>(value, 0, value)</returns>
        public static Vector3 non(this float value) => new(value, 0f, value);
        /// <returns>(0, value, value)</returns>
        public static Vector3 onn(this float value) => new(0f, value, value);
        /// <returns>(value, value)</returns>
        [Obsolete("Please use StickV2() instead.")] public static Vector2 nn(this float value) => new(value, value);
        /// <returns>(value, value, value)</returns>
        [Obsolete("Please use StickV3() instead.")] public static Vector3 nnn(this float value) => new(value, value, value);

        public static Vector2 Make(this in (float x, float y) tuple) => new(tuple.x, tuple.y);
        public static Vector2Int Make(this in (int x, int y) tuple) => new(tuple.x, tuple.y);
        public static Vector3 Make(this in (float x, float y, float z) tuple) => new(tuple.x, tuple.y, tuple.z);
        public static Vector3Int Make(this in (int x, int y, int z) tuple) => new(tuple.x, tuple.y, tuple.z);
        #endregion

        #region Angle
        /// <returns>the angle of (x, y) from axis +x</returns>
        public static float ToRadians(float x, float y) => Mathf.Atan2(y, x);
        /// <returns>the angle of (x, y) from axis +x</returns>
        public static float ToDegrees(float x, float y) => Mathf.Atan2(y, x) * Mathf.Rad2Deg;
        public static Vector2 RadiansToVector2(this float value) => new(Mathf.Cos(value), Mathf.Sin(value));
        public static Vector2 DegreesToVector2(this float value) => RadiansToVector2(Mathf.Deg2Rad * value);
        #endregion
    }
}
