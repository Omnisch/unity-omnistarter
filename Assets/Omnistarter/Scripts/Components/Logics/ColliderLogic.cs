// author: Omnistudio
// version: 2025.11.23

using UnityEngine;
using UnityEngine.Events;

namespace Omnis
{
    [RequireComponent(typeof(Collider))]
    public class ColliderLogic : MonoBehaviour
    {
        #region Serialized fields
        public UnityEvent collisionEnterCallback;
        public UnityEvent collisionExitCallback;
        #endregion

        #region Unity methods
        private void OnCollisionEnter(Collision collision)
        {
            collisionEnterCallback?.Invoke();
        }
        private void OnCollisionExit(Collision collision)
        {
            collisionExitCallback?.Invoke();
        }
        private void OnTriggerEnter(Collider other)
        {
            collisionEnterCallback?.Invoke();
        }
        private void OnTriggerExit(Collider other)
        {
            collisionExitCallback?.Invoke();
        }
        #endregion
    }
}
