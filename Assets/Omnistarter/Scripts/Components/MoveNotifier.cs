// author: Omnistudio
// version: 2025.11.06

using UnityEngine;
using UnityEngine.Events;

namespace Omnis
{
    public class MoveNotifier : MonoBehaviour
    {
        private UnityAction<Vector3> del;
        private Vector3 lastPosition;
        private Vector3 LastPosition {
            get => lastPosition;
            set {
                if (lastPosition != value) {
                    lastPosition = value;
                    del?.Invoke(value);
                }
            }
        }

        public void Subscribe(UnityAction<Vector3> action) {
            del += action;
            action(LastPosition);
        }
        public void Unsubscribe(UnityAction<Vector3> action) => del -= action;


        private void Update()
            => LastPosition = transform.position;
    }
}
