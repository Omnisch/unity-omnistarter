// author: Omnistudio
// version: 2026.02.01

using UnityEngine;

namespace Omnis
{
    public sealed class AlwaysLookAt : MonoBehaviour
    {
        public Vector3 localEye;
        public Vector3 localUp;
        public Transform target;
        public bool lookAtCamera;

        private void LateUpdate()
        {
            var actualTarget = lookAtCamera ? (Camera.main ? Camera.main.transform : null) : target;
            if (actualTarget == null)
                return;

            transform.LookAt(actualTarget);
            transform.localRotation *= Quaternion.LookRotation(localEye, localUp);
        }
    }
}
