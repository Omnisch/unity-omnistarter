// author: Omnistudio
// version: 2026.01.08

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Omnis
{
    /// <summary>
    /// Hooked with .inputactions in the new Input System.
    /// </summary>
    [RequireComponent(typeof(PlayerInput))]
    public class InputHandler : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] protected UnityEvent debugLogic;

        [Header("Cursor Settings")]
        [SerializeField] private Texture2D iconCursor;
        [SerializeField] private Texture2D iconCursorPressed;
        [SerializeField] private Vector2 cursorHotspot = new(16f, 16f);
        #endregion

        #region Fields
        private PlayerInput playerInput;
        private List<Collider> hits = new();
        /// <summary>
        /// Used to handle LeftPressed-then-PointerOutOfBounds situations, in ReleaseLeftOOBs().
        /// </summary>
        private List<Collider> hitsLeftTrack = new();
        private List<GameObject> listeners = new();
        #endregion

        #region Properties
        private Vector2 pointerPosition;
        private Vector2 PointerPosition
        {
            get => pointerPosition;
            set {
                pointerPosition = value;
                Ray r = Camera.main.ScreenPointToRay(value);
                var rawHits = Physics.RaycastAll(r);
                System.Array.Sort(rawHits, (a, b) => a.distance.CompareTo(b.distance));
                List<Collider> newHits = rawHits.Select(hit => hit.collider).ToList();
                foreach (var hit in hits.Except(newHits).ToList())
                    if (hit) {
                        hit.SendMessage("OnPointerExit", options: SendMessageOptions.DontRequireReceiver);
                        if (hit.TryGetComponent<PointerReceiver>(out var pr) && pr.opaque) break;
                    }
                foreach (var hit in newHits.Except(hits).ToList())
                    if (hit) {
                        hit.SendMessage("OnPointerEnter", options: SendMessageOptions.DontRequireReceiver);
                        if (hit.TryGetComponent<PointerReceiver>(out var pr) && pr.opaque) break;
                    }
                hits = newHits;
            }
        }
        private Texture2D CursorIcon
        {
            set => Cursor.SetCursor(value, cursorHotspot, CursorMode.Auto);
        }
        #endregion

        #region Public Functions
        public void AddListener(GameObject listener) => listeners.Add(listener);
        public bool RemoveListener(GameObject listener) => listeners.Remove(listener);
        #endregion

        #region Functions
        protected void ForwardMessageToHits(string methodName, object value = null)
        {
            // manually refresh hits to avoid resting.
            PointerPosition = PointerPosition;
            
            foreach (var hit in hits)
            {
                if (hit && hit.TryGetComponent<PointerReceiver>(out var pr))
                {
                    hit.SendMessage("OnInteract", hits, SendMessageOptions.DontRequireReceiver);
                    hit.SendMessage(methodName, value, SendMessageOptions.DontRequireReceiver);
                    // if opaque, ignore colliders behind it.
                    if (pr.opaque) break;
                }
            }
        }
        protected void ForwardMessageToListeners(string methodName, object value = null)
        {
            foreach (var listener in listeners)
            {
                listener.SendMessage("OnInteract", listeners, SendMessageOptions.DontRequireReceiver);
                listener.SendMessage(methodName, value, SendMessageOptions.DontRequireReceiver);
            }
        }

        protected void ReleaseLeftOOBs()
        {
            foreach (var hit in hitsLeftTrack)
            {
                if (hit && hit.TryGetComponent<PointerReceiver>(out var pr))
                {
                    if (pr.TryGetComponent<PointerBase>(out var pb) && pb.LeftPressed) {
                        hit.SendMessage("OnInteract", hits, SendMessageOptions.DontRequireReceiver);
                        hit.SendMessage("OnLeftRelease", SendMessageOptions.DontRequireReceiver);
                    }
                    if (pr.opaque) break;
                }
            }
        }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();

            foreach (var map in playerInput.actions.actionMaps)
                map.Enable();
        }
        private void OnEnable()
        {
            if (playerInput) playerInput.enabled = true;
            Cursor.visible = true;
        }
        private void OnDisable()
        {
            if (playerInput) playerInput.enabled = false;
            Cursor.visible = false;
        }
        #endregion

        #region Messages
        private void OnLeftPress()
        {
            ForwardMessageToHits("OnLeftPress");
            hitsLeftTrack = hits;
            CursorIcon = iconCursorPressed;
        }
        private void OnLeftRelease()
        {
            ForwardMessageToHits("OnLeftRelease");
            ReleaseLeftOOBs();
            CursorIcon = iconCursor;
        }
        private void OnRightPress() => ForwardMessageToHits("OnRightPress");
        private void OnRightRelease() => ForwardMessageToHits("OnRightRelease");
        private void OnMiddlePress() => ForwardMessageToHits("OnMiddlePress");
        private void OnMiddleRelease() => ForwardMessageToHits("OnMiddleRelease");
        private void OnScroll(InputValue value) => ForwardMessageToHits("OnScroll", value.Get<float>());
        private void OnPointer(InputValue value) => PointerPosition = value.Get<Vector2>();

        private void OnMove(InputValue value) => ForwardMessageToListeners("OnMove", value.Get<Vector2>());
        private void OnJump(InputValue value) => ForwardMessageToListeners("OnJump", value.Get<float>());
        private void OnAct() => ForwardMessageToListeners("OnAct");
        private void OnCrouch() => ForwardMessageToListeners("OnCrouch");
        private void OnUndo() => ForwardMessageToListeners("OnUndo");
        private void OnRetry() => ForwardMessageToListeners("OnRetry");

        private void OnSave() => ForwardMessageToHits("OnSave");
        private void OnLoad() => ForwardMessageToHits("OnLoad");
        private void OnDebugTest() => debugLogic?.Invoke();
        private void OnEscape()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            Application.Quit();
#endif
        }
        #endregion
    }
}
