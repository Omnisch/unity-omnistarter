// author: Omnistudio
// version: 2025.11.06

using UnityEngine;

namespace Omnis
{
    [RequireComponent(typeof(Canvas))]
    public class CanvasRenderModeSwitch : MonoBehaviour
    {
        [SerializeField] private Transform worldParent;
        private Canvas canvas;

        public void SwitchRenderMode() {
            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                canvas.renderMode = RenderMode.WorldSpace;
                canvas.worldCamera = Camera.main;
                transform.SetParent(worldParent);
                transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                transform.localScale = Vector3.one;
            } else if (canvas.renderMode == RenderMode.WorldSpace) {
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
        }

        private void Start() {
            if (TryGetComponent<Canvas>(out var c)) {
                canvas = c;
            } else {
                Destroy(this);
            }
        }
    }
}
