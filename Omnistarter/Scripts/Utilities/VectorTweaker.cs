// author: Omnistudio
// version: 2025.02.16

using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// Auxiliary functions of UnityEngine.Vector2 and UnityEngine.Vector3.
    /// </summary>
    public abstract class VectorTweaker
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
        public static float V2ToRadians(Vector2 v2) => Mathf.Atan2(v2.y, v2.x);
        public static float V2ToDegrees(Vector2 v2) => Mathf.Atan2(v2.y, v2.x) * Mathf.Rad2Deg;
        #endregion

        #region Vector2 to Vector3
        /// <returns>(x, y, n)</returns>
        public static Vector3 V2ToV3xy(Vector2 v2, float n) => new(v2.x, v2.y, n);
        /// <returns>(x, n, y)</returns>
        public static Vector3 V2ToV3xz(Vector2 v2, float n) => new(v2.x, n, v2.y);
        /// <returns>(n, x, y)</returns>
        public static Vector3 V2ToV3yz(Vector2 v2, float n) => new(n, v2.x, v2.y);
        /// <returns>(x, y, 0)</returns>
        public static Vector3 V2ToV3xy(Vector2 v2) => new(v2.x, v2.y, 0f);
        /// <returns>(x, 0, y)</returns>
        public static Vector3 V2ToV3xz(Vector2 v2) => new(v2.x, 0f, v2.y);
        /// <returns>(0, x, y)</returns>
        public static Vector3 V2ToV3yz(Vector2 v2) => new(0f, v2.x, v2.y);
        #endregion

        #region Vector3 to Vector2
        /// <returns>(x, y)</returns>
        public static Vector2 xy(Vector3 v3) => new(v3.x, v3.y);
        /// <returns>(x, z)</returns>
        public static Vector2 xz(Vector3 v3) => new(v3.x, v3.z);
        /// <returns>(y, z)</returns>
        public static Vector2 yz(Vector3 v3) => new(v3.y, v3.z);
        #endregion

        #region Change one value in Vector3
        /// <returns>(x, y, n)</returns>
        public static Vector3 xyn(Vector3 v, float n) => new(v.x, v.y, n);
        /// <returns>(x1, y1, z2)</returns>
        public static Vector3 xyn(Vector3 v1, Vector3 v2) => new(v1.x, v1.y, v2.z);
        /// <returns>(x, n, z)</returns>
        public static Vector3 xnz(Vector3 v, float n) => new(v.x, n, v.z);
        /// <returns>(x1, y2, z1)</returns>
        public static Vector3 xnz(Vector3 v1, Vector3 v2) => new(v1.x, v2.y, v1.z);
        /// <returns>(n, y, z)</returns>
        public static Vector3 nyz(Vector3 v, float n) => new(n, v.y, v.z);
        /// <returns>(x2, y1, z1)</returns>
        public static Vector3 nyz(Vector3 v1, Vector3 v2) => new(v2.x, v1.y, v1.z);
        /// <returns>(x, y, 0)</returns>
        public static Vector3 xyo(Vector3 v) => new(v.x, v.y, 0f);
        /// <returns>(x, 0, y)</returns>
        public static Vector3 xoz(Vector3 v) => new(v.x, 0f, v.z);
        /// <returns>(0, y, z)</returns>
        public static Vector3 oyz(Vector3 v) => new(0f, v.y, v.z);
        #endregion

        #region Round
        public static Vector3 Round(Vector3 v) => new(Mathf.Round(v.x), Mathf.Round(v.y), Mathf.Round(v.z));
        public static Vector3 GridSnap(Vector3 v, float increment = 1f) => increment == 0f ? v : increment * Round(v / increment);
        #endregion
    }
}
