// author: Omnistudio
// version: 2025.09.03

using UnityEngine;

namespace Omnis
{
    public sealed class AlwaysLookAt : MonoBehaviour
    {
        [SerializeField] private Vector3 localEye;
        [SerializeField] private Vector3 localUp;
        [SerializeField] private Transform target;
        [SerializeField] private bool lookAtCamera;

        private void LateUpdate()
        {
            var actualTarget = this.lookAtCamera ? (Camera.main ? Camera.main.transform : null) : this.target;
            if (actualTarget == null)
                return;

            this.transform.LookAt(actualTarget);
            this.transform.localRotation *= Quaternion.LookRotation(this.localEye, this.localUp);
        }
    }
}
