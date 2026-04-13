// author: Omnistudio
// version: 2026.04.13

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
    public class InputRouter : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] protected UnityEvent debugLogic;

        [Header("Cursor Settings")]
        [SerializeField] private Texture2D iconCursor;
        [SerializeField] private Texture2D iconCursorPressed;
        [SerializeField] private Vector2 cursorHotspot = new(16f, 16f);
        #endregion

        #region Fields
        protected PlayerInput playerInput;
        private List<Collider> hits = new();
        private readonly RaycastHit[] rawHits = new RaycastHit[256];
        /// <summary>
        /// Used to handle LeftPressed-then-PointerOutOfBounds situations, in ReleaseLeftOOBs().
        /// </summary>
        private List<Collider> hitsLeftTrack = new();
        private readonly List<GameObject> listeners = new();
        #endregion

        #region Properties
        public static InputRouter Instance { get; private set; }
        
        private Vector2 pointerPosition;
        public Vector2 PointerPosition {
            get => pointerPosition;
            private set {
                pointerPosition = value;

                if (Camera.main == null) return;
                
                Ray r = Camera.main.ScreenPointToRay(value);
                int hitCount = Physics.RaycastNonAlloc(r, rawHits);
                
                System.Array.Sort(rawHits, 0, hitCount, new RaycastHitDistanceComparer());
                var newHits = rawHits[..hitCount].Select(h => h.collider).ToList();
                
                foreach (var col in hits.Except(newHits).Where(c => c != null)) {
                    col.SendMessage("OnPointerExitRaw", options: SendMessageOptions.DontRequireReceiver);
                    if (col.TryGetComponent<PointerReceiver>(out var pr) && pr.opaque) break;
                }
                
                foreach (var col in newHits.Except(hits).Where(c => c != null)) {
                    col.SendMessage("OnPointerEnterRaw", options: SendMessageOptions.DontRequireReceiver);
                    if (col.TryGetComponent<PointerReceiver>(out var pr) && pr.opaque) break;
                }
                
                hits = newHits;
            }
        }
        private sealed class RaycastHitDistanceComparer : IComparer<RaycastHit>
        {
            public int Compare(RaycastHit a, RaycastHit b) {
                return a.distance.CompareTo(b.distance);
            }
        }
        
        public Vector2 PointerDelta { get; private set; }

        protected Texture2D CursorIcon {
            set => Cursor.SetCursor(value, cursorHotspot, CursorMode.Auto);
        }
        #endregion

        #region Public Functions
        public void AddListener(GameObject listener) => listeners.Add(listener);
        public bool RemoveListener(GameObject listener) => listeners.Remove(listener);
        #endregion

        #region Functions
        protected void ForwardMessageToHits(string methodName, object value = null) {
            // manually refresh hits to avoid resting.
            PointerPosition = PointerPosition;

            foreach (var hit in hits) {
                if (hit && hit.TryGetComponent<PointerReceiver>(out var pr)) {
                    hit.SendMessage("OnInteractRaw", hits, SendMessageOptions.DontRequireReceiver);
                    hit.SendMessage(methodName, value, SendMessageOptions.DontRequireReceiver);
                    // if opaque, ignore colliders behind it.
                    if (pr.opaque) break;
                }
            }
        }
        protected void ForwardMessageToListeners(string methodName, object value = null) {
            foreach (var listener in listeners) {
                listener.SendMessage("OnInteract", listeners, SendMessageOptions.DontRequireReceiver);
                listener.SendMessage(methodName, value, SendMessageOptions.DontRequireReceiver);
            }
        }

        protected void ReleaseLeftOOBs() {
            foreach (var hit in hitsLeftTrack) {
                if (hit && hit.TryGetComponent<PointerReceiver>(out var pr)) {
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
        protected virtual void Awake() {
            Instance = this;
            
            playerInput = GetComponent<PlayerInput>();

            foreach (var map in playerInput.actions.actionMaps)
                map.Enable();
        }
        protected virtual void OnEnable() {
            if (playerInput)
                playerInput.enabled = true;
            
            Cursor.visible = true;
        }
        protected virtual void OnDisable() {
            if (playerInput)
                playerInput.enabled = false;
            
            Cursor.visible = false;
        }
        #endregion

        #region Messages
        protected virtual void OnLeftPress() {
            ForwardMessageToHits("OnLeftPressRaw");
            hitsLeftTrack = hits;
            CursorIcon = iconCursorPressed;
        }
        protected virtual void OnLeftRelease() {
            ForwardMessageToHits("OnLeftReleaseRaw");
            ReleaseLeftOOBs();
            CursorIcon = iconCursor;
        }
        protected virtual void OnRightPress() => ForwardMessageToHits("OnRightPressRaw");
        protected virtual void OnRightRelease() => ForwardMessageToHits("OnRightReleaseRaw");
        protected virtual void OnMiddlePress() => ForwardMessageToHits("OnMiddlePressRaw");
        protected virtual void OnMiddleRelease() => ForwardMessageToHits("OnMiddleReleaseRaw");
        protected virtual void OnScroll(InputValue value) => ForwardMessageToHits("OnScrollRaw", value.Get<float>());
        protected virtual void OnPointer(InputValue value) => PointerPosition = value.Get<Vector2>();
        protected virtual void OnPointerDelta(InputValue value) => PointerDelta = value.Get<Vector2>();

        protected virtual void OnMove(InputValue value) => ForwardMessageToListeners("OnMove", value.Get<Vector2>());
        protected virtual void OnJump(InputValue value) => ForwardMessageToListeners("OnJump", value.Get<float>());
        protected virtual void OnAct() => ForwardMessageToListeners("OnAct");
        protected virtual void OnCrouch() => ForwardMessageToListeners("OnCrouch");
        protected virtual void OnUndo() => ForwardMessageToListeners("OnUndo");
        protected virtual void OnRetry() => ForwardMessageToListeners("OnRetry");

        protected virtual void OnSave() => ForwardMessageToListeners("OnSave");
        protected virtual void OnLoad() => ForwardMessageToListeners("OnLoad");
        protected virtual void OnDebugTest() => debugLogic?.Invoke();
        protected virtual void OnQuitGame() {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
            Application.Quit();
#endif
        }
        #endregion
    }
}
