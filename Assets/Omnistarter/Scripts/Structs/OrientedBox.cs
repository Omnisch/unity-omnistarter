// author: Omnistudio
// version: 2025.09.03

using UnityEngine;

namespace Omnis
{
    public struct OrientedBox
    {
        public Vector3 center;
        public Quaternion rotation;
        public Vector3 halfExtents;

        /// <summary>
        /// Use a diagonal (pDiag1 and pDiag2), upNormal and a vertex pRef to create an OBB.<br/>
        /// pRef should be at the bottom face, adjacent to pDiag1.
        /// </summary>
        public static OrientedBox FromDiagonalAndRef(
            Vector3 pDiag1,
            Vector3 pDiag2,
            Vector3 pRef,
            Vector3 upNormal,
            bool asPlane = false,
            float minThicknessY = 0f)
        {
            Vector3 c = 0.5f * (pDiag1 + pDiag2);
            Vector3 d = pDiag2 - pDiag1;
            Vector3 up = upNormal.normalized;

            Vector3 forward = pRef - pDiag1;
            // fallback
            if (forward.sqrMagnitude < 1e-8f) {
                forward = Vector3.ProjectOnPlane(d, up);
                if (forward.sqrMagnitude < 1e-8f) {
                    forward = Vector3.Cross(up, Vector3.right);
                    if (forward.sqrMagnitude < 1e-8f) forward = Vector3.Cross(up, Vector3.forward);
                }
            }
            forward.Normalize();

            Vector3 right = Vector3.Cross(up, forward).normalized;
            Vector3.OrthoNormalize(ref up, ref forward, ref right);

            float hx = 0.5f * Mathf.Abs(Vector3.Dot(d, right));
            float hy_raw = 0.5f * Mathf.Abs(Vector3.Dot(d, up));
            float hz = 0.5f * Mathf.Abs(Vector3.Dot(d, forward));

            float hy = asPlane ? Mathf.Max(minThicknessY, 0f)
                               : Mathf.Max(minThicknessY, hy_raw);

            return new OrientedBox
            {
                center = c,
                rotation = Quaternion.LookRotation(forward, up),
                halfExtents = new Vector3(hx, hy, hz)
            };
        }

        public readonly bool Contains(Vector3 point, float epsilon = 1e-5f)
        {
            Vector3 local = Quaternion.Inverse(rotation) * (point - center);
            return Mathf.Abs(local.x) <= halfExtents.x + epsilon
                && Mathf.Abs(local.y) <= halfExtents.y + epsilon
                && Mathf.Abs(local.z) <= halfExtents.z + epsilon;
        }

        public readonly bool ContainsOnPlane(Vector3 point, float planeEps = 1e-4f)
        {
            Vector3 local = Quaternion.Inverse(rotation) * (point - center);
            bool onPlane = Mathf.Abs(local.y) <= planeEps;
            return onPlane
                && Mathf.Abs(local.x) <= halfExtents.x
                && Mathf.Abs(local.z) <= halfExtents.z;
        }

        public readonly Vector3 Clamp(Vector3 point)
        {
            Vector3 local = Quaternion.Inverse(rotation) * (point - center);

            Vector3 clampedLocal = new(
                Mathf.Clamp(local.x, -halfExtents.x, halfExtents.x),
                Mathf.Clamp(local.y, -halfExtents.y, halfExtents.y),
                Mathf.Clamp(local.z, -halfExtents.z, halfExtents.z)
            );

            return center + rotation * clampedLocal;
        }
    }
}
