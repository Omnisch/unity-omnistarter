// author: Omnistudio
// version: 2025.11.06

using Omnis.Utils;
using UnityEngine;

namespace Omnis
{
    public class ChangeParent : MonoBehaviour
    {
        [SerializeField] private Transform[] targets;
        [SerializeField] private int targetIndex = 0;
        [Space]
        [SerializeField] private float lerpTime = 0.3f;

        private Coroutine moveCoroutine = null;

        public void MoveToNextParent() => SetParent((targetIndex + 1) % targets.Length);
        public void SetParent(int i) {
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

            moveCoroutine = StartCoroutine(YieldHelper.Ease((value) => {
                transform.SetPositionAndRotation(
                    Vector3.Lerp(oldPosition, targets[targetIndex].position, value),
                    Quaternion.Lerp(oldRotation, targets[targetIndex].rotation, value));
                VectorHelper.SetGlobalScale(transform, Vector3.Lerp(oldScale, targets[targetIndex].lossyScale, value));

                if (value == 1f) {
                    moveCoroutine = null;
                    transform.SetParent(targets[targetIndex]);
                    transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                    transform.localScale = Vector3.one;
                }
            }, Easing.OutCubic, lerpTime, false));
        }
    }
}
