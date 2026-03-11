// author: Omnistudio
// version: 2026.03.11

using Omnis.Utils;
using OmnisEditor;
using UnityEngine;

namespace Omnis
{
    [ExecuteAlways]
    public class ChangeParent : MonoBehaviour
    {
        [SerializeField] private Transform[] targets;
        [SerializeField] private int targetIndex = 0;
        [Header("Animation")]
        public float lerpTime = 0.3f;
        [ConditionalGroup]
        public EasingSettings easing = new(Easing.EasingType.CubicOut);

        private Coroutine moveCoroutine = null;

        [ContextMenu("Move To Next Parent")]
        public void MoveToNextParent() {
            if (targets != null && targets.Length > 0)
                SetParent((targetIndex + 1) % targets.Length);
        }
        public void SetParent(int i) => SetParent(i, null);
        public void SetParent(int i, System.Action callback) {
            if (i < 0 || i >= targets.Length) {
                Debug.LogWarning("Manual-set index out of bounds.");
                return;
            }

            targetIndex = i;

            if (moveCoroutine != null) {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }

            transform.GetPositionAndRotation(out var oldPosition, out var oldRotation);
            var oldScale = transform.lossyScale;

            moveCoroutine = this.Ease((value) => {
                transform.SetPositionAndRotation(
                    Vector3.Lerp(oldPosition, targets[targetIndex].position, value),
                    Quaternion.Lerp(oldRotation, targets[targetIndex].rotation, value));
                VectorHelper.SetGlobalScale(transform, Vector3.Lerp(oldScale, targets[targetIndex].lossyScale, value));

                if (value == 1f) {
                    moveCoroutine = null;
                    transform.SetParent(targets[targetIndex]);
                    transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    transform.localScale = Vector3.one;
                    callback?.Invoke();
                }
            }, easing.Evaluate, lerpTime, false);
        }
    }
}
