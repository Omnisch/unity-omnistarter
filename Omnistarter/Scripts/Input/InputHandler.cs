// author: Omnistudio
// version: 2025.03.24

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
        #endregion

        #region Fields
        private PlayerInput playerInput;
        private List<Collider> hits;
        private List<GameObject> listeners;
        #endregion

        #region Properties
        public static Vector2 PointerPosition {  get; private set; }
        #endregion

        #region Public Functions
        public void AddListener(GameObject listener) => listeners.Add(listener);
        public bool RemoveListener(GameObject listener) => listeners.Remove(listener);
        #endregion

        #region Functions
        protected void ForwardMessageToHits(string methodName, object value = null)
        {
            foreach (var hit in hits)
            {
                if (!hit.GetComponent<PointerBase>()) continue;
                hit.SendMessage("OnInteract", hits, SendMessageOptions.DontRequireReceiver);
                hit.SendMessage(methodName, value, SendMessageOptions.DontRequireReceiver);
                if (hit.GetComponent<PointerBase>() && hit.GetComponent<PointerBase>().opaque) break;
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
        #endregion

        #region Unity Methods
        private void Awake()
        {
            playerInput = GetComponent<PlayerInput>();

            foreach (var map in playerInput.actions.actionMaps)
                map.Enable();

            hits = new();
            listeners = new();
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
        private void OnLeftPress() => ForwardMessageToHits("OnLeftPress");
        private void OnLeftRelease() => ForwardMessageToHits("OnLeftRelease");
        private void OnRightPress() => ForwardMessageToHits("OnRightPress");
        private void OnRightRelease() => ForwardMessageToHits("OnRightRelease");
        private void OnMiddlePress() => ForwardMessageToHits("OnMiddlePress");
        private void OnMiddleRelease() => ForwardMessageToHits("OnMiddleRelease");
        private void OnScroll(InputValue value) => ForwardMessageToHits("OnScroll", value.Get<float>());
        private void OnPointer(InputValue value)
        {
            PointerPosition = value.Get<Vector2>();
            Ray r = Camera.main.ScreenPointToRay(value.Get<Vector2>());
            var rawHits = Physics.RaycastAll(r);
            System.Array.Sort(rawHits, (a, b) => a.distance.CompareTo(b.distance));
            List<Collider> newHits = rawHits.Select(hit => hit.collider).ToList();
            foreach (var hit in hits.Except(newHits).ToList())
                if (hit)
                {
                    hit.SendMessage("OnPointerExit", options: SendMessageOptions.DontRequireReceiver);
                    if (hit.GetComponent<PointerBase>() && hit.GetComponent<PointerBase>().opaque) break;
                }
            foreach (var hit in newHits.Except(hits).ToList())
                if (hit)
                {
                    hit.SendMessage("OnPointerEnter", options: SendMessageOptions.DontRequireReceiver);
                    if (hit.GetComponent<PointerBase>() && hit.GetComponent<PointerBase>().opaque) break;
                }
            hits = newHits;
        }

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
#if UNITY_STANDALONE
            Application.Quit();
#elif UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
        #endregion
    }
}
