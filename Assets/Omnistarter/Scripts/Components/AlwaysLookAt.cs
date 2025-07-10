// author: Omnistudio
// version: 2025.07.10

using UnityEngine;

namespace Omnis
{
    public class AlwaysLookAt : MonoBehaviour
    {
        [SerializeField] private Vector3 localEye;
        [SerializeField] private Vector3 localUp;
        [SerializeField] private Transform target;
        [SerializeField] private bool lookAtCamera;

        private void LateUpdate()
        {
            var actualTarget = lookAtCamera ? (Camera.current ? Camera.current.transform : null) : target;
            if (actualTarget == null)
                return;

            transform.LookAt(actualTarget);
            transform.localRotation *= Quaternion.LookRotation(localEye, localUp);
        }
    }
}
