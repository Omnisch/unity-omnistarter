// author: Omnistudio
// version: 2025.09.22

using System;
using UnityEngine;

namespace Omnis
{
    public class LocationSwitch : MonoBehaviour
    {
        [SerializeField] private bool isLocal = false;
        [SerializeField] private TransformInfo[] locations;
        [SerializeField] private int locationIndex = 0;
        [Space]
        [SerializeField] private float lerpTime = 0.3f;

        private Coroutine moveCoroutine = null;

        public void NextLocation() => SetLocation((locationIndex + 1) % locations.Length);
        public void SetLocation(int i) {
            if (i < 0 || i >= locations.Length) {
                Debug.LogWarning("Manual-set index out of bounds.");
                return;
            }

            locationIndex = i;

            if (moveCoroutine != null) {
                StopCoroutine(moveCoroutine);
                moveCoroutine = null;
            }

            var oldPosition = isLocal ? transform.localPosition : transform.position;
            var oldEulerAngles = isLocal ? transform.localRotation.eulerAngles : transform.rotation.eulerAngles;
            var oldScale = isLocal ? transform.localScale : transform.lossyScale;

            moveCoroutine = StartCoroutine(Utils.YieldHelper.Ease((value) => {
                if (isLocal) {
                    transform.localPosition = Vector3.Lerp(oldPosition, locations[locationIndex].Position, value);
                    transform.localEulerAngles = Vector3.Lerp(oldEulerAngles, locations[locationIndex].Rotation, value);
                    transform.localScale = Vector3.Lerp(oldScale, locations[locationIndex].Scale, value);
                } else {
                    transform.position = Vector3.Lerp(oldPosition, locations[locationIndex].Position, value);
                    transform.eulerAngles = Vector3.Lerp(oldEulerAngles, locations[locationIndex].Rotation, value);
                    SetGlobalScale(transform, Vector3.Lerp(oldScale, locations[locationIndex].Scale, value));
                }

                if (value == 1f) {
                    moveCoroutine = null;
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


        [Serializable]
        public struct TransformInfo {
            public Vector3 Position;
            public Vector3 Rotation;
            public Vector3 Scale;
        }
    }
}
