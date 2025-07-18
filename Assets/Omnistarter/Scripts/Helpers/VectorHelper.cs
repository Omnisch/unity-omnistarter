// author: Omnistudio
// version: 2025.07.18

using UnityEngine;

namespace Omnis.Utils
{
    /// <summary>
    /// Auxiliary methods of UnityEngine.Vector2 and UnityEngine.Vector3.
    /// </summary>
    public static class VectorHelper
    {
        #region Vector2 consts
        /// <summary>Shorthand for writing Vector2(-1, -1).</summary>
        public static Vector2 lb => new(-1f, -1f);
        /// <summary>Shorthand for writing Vector2(-1, 1).</summary>
        public static Vector2 lt => new(-1f, 1f);
        /// <summary>Shorthand for writing Vector2(1, -1).</summary>
        public static Vector2 rb => new(1f, -1f);
        /// <summary>Shorthand for writing Vector2(1, 1).</summary>
        public static Vector2 rt => new(1f, 1f);
        #endregion

        #region Vector2 to angle
        public static float ToRadians(this Vector2 v2) => Mathf.Atan2(v2.y, v2.x);
        public static float ToDegrees(this Vector2 v2) => Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        #endregion

        #region Vector2 to Vector3
        /// <returns>(x, y, n)</returns>
        public static Vector3 xyn(this Vector2 v2, float n) => new(v2.x, v2.y, n);
        /// <returns>(x, n, y)</returns>
        public static Vector3 xny(this Vector2 v2, float n) => new(v2.x, n, v2.y);
        /// <returns>(n, x, y)</returns>
        public static Vector3 nxy(this Vector2 v2, float n) => new(n, v2.x, v2.y);
        /// <returns>(x, y, 0)</returns>
        public static Vector3 xyo(this Vector2 v2) => new(v2.x, v2.y, 0f);
        /// <returns>(x, 0, y)</returns>
        public static Vector3 xoy(this Vector2 v2) => new(v2.x, 0f, v2.y);
        /// <returns>(0, x, y)</returns>
        public static Vector3 oxy(this Vector2 v2) => new(0f, v2.x, v2.y);
        #endregion

        #region Vector3 to Vector2
        /// <returns>(x, y)</returns>
        public static Vector2 xy(this Vector3 v3) => new(v3.x, v3.y);
        /// <returns>(x, z)</returns>
        public static Vector2 xz(this Vector3 v3) => new(v3.x, v3.z);
        /// <returns>(y, z)</returns>
        public static Vector2 yz(this Vector3 v3) => new(v3.y, v3.z);
        #endregion

        #region Change one value in Vector3
        /// <returns>(x, y, n)</returns>
        public static Vector3 xyn(this Vector3 v, float n) => new(v.x, v.y, n);
        /// <returns>(x1, y1, z2)</returns>
        public static Vector3 xyn(this Vector3 v1, Vector3 v2) => new(v1.x, v1.y, v2.z);
        /// <returns>(x, n, z)</returns>
        public static Vector3 xnz(this Vector3 v, float n) => new(v.x, n, v.z);
        /// <returns>(x1, y2, z1)</returns>
        public static Vector3 xnz(this Vector3 v1, Vector3 v2) => new(v1.x, v2.y, v1.z);
        /// <returns>(n, y, z)</returns>
        public static Vector3 nyz(this Vector3 v, float n) => new(n, v.y, v.z);
        /// <returns>(x2, y1, z1)</returns>
        public static Vector3 nyz(this Vector3 v1, Vector3 v2) => new(v2.x, v1.y, v1.z);
        /// <returns>(x, y, 0)</returns>
        public static Vector3 xyo(this Vector3 v) => new(v.x, v.y, 0f);
        /// <returns>(x, 0, y)</returns>
        public static Vector3 xoz(this Vector3 v) => new(v.x, 0f, v.z);
        /// <returns>(0, y, z)</returns>
        public static Vector3 oyz(this Vector3 v) => new(0f, v.y, v.z);
        #endregion

        #region Abs
        public static Vector2 Abs(this Vector2 v) => new(v.x.Abs(), v.y.Abs());
        public static Vector3 Abs(this Vector3 v) => new(v.x.Abs(), v.y.Abs(), v.z.Abs());
        #endregion

        #region Clamp
        public static Vector3 Clamp(this Vector3 v, Vector3 min, Vector3 max)
        {
            float x = v.x.Clamp(min.x, max.x);
            float y = v.y.Clamp(min.y, max.y);
            float z = v.z.Clamp(min.z, max.z);
            return new Vector3(x, y, z);
        }
        #endregion

        #region Comparison
        public static bool AllLessThan(this Vector3 v, Vector3 other)
            => v.x < other.x && v.y < other.y && v.z < other.z;
        public static bool AllLessThanOrEqualsTo(this Vector3 v, Vector3 other)
            => v.x <= other.x && v.y <= other.y && v.z <= other.z;
        public static bool AllGreaterThan(this Vector3 v, Vector3 other)
            => v.x > other.x && v.y > other.y && v.z > other.z;
        public static bool AllGreaterThanOrEqualsTo(this Vector3 v, Vector3 other)
            => v.x >= other.x && v.y >= other.y && v.z >= other.z;
        #endregion

        #region Flip
        /// <summary>Lets x be y and y be x.</summary>
        public static Vector2 Flip(this Vector2 v) => new(v.y, v.x);
        /// <summary>Lets x be z and z be x.</summary>
        public static Vector3 Flip(this Vector3 v) => new(v.z, v.y, v.x);
        #endregion

        #region Round
        public static Vector3 Round(this Vector3 v) => new(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
        public static Vector3 GridSnap(this Vector3 v, float increment = 1f) => increment == 0f ? v : increment * Round(v / increment);
        public static Vector3 RoundDigits(this Vector3 v, int digits)
            => new((float)System.Math.Round(v.x, digits), (float)System.Math.Round(v.y, digits), (float)System.Math.Round(v.z, digits));
        #endregion

    }
}
