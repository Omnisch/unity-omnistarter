// author: Omnistudio
// version: 2025.09.26

using UnityEngine;

namespace Omnis
{
    public class ParentSwitch : MonoBehaviour
    {
        [SerializeField] private Transform[] targets;
        [SerializeField] private int targetIndex = 0;
        [Space]
        [SerializeField] private float lerpTime = 0.3f;

        private Coroutine moveCoroutine = null;

        public void NextLocation() => SetLocation((targetIndex + 1) % targets.Length);
        public void SetLocation(int i) {
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

            moveCoroutine = StartCoroutine(Utils.YieldHelper.Ease((value) => {
                transform.SetPositionAndRotation(
                    Vector3.Lerp(oldPosition, targets[targetIndex].position, value),
                    Quaternion.Lerp(oldRotation, targets[targetIndex].rotation, value));
                SetGlobalScale(transform, Vector3.Lerp(oldScale, targets[targetIndex].lossyScale, value));

                if (value == 1f) {
                    moveCoroutine = null;
                    transform.parent = targets[targetIndex];
                }
            }, Utils.Easing.OutCubic, lerpTime, false));
        }

        private void SetGlobalScale(Transform t, Vector3 globalScale) {
            t.localScale = new Vector3(
                globalScale.x / t.lossyScale.x * t.localScale.x,
                globalScale.y / t.lossyScale.y * t.localScale.y,
                globalScale.z / t.lossyScale.z * t.localScale.z
            );
        }
    }
}
