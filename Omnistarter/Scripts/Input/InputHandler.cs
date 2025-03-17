// author: Omnistudio
// version: 2025.03.17

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
        protected virtual void OnLeftPress() => ForwardMessageToHits("OnLeftPress");
        protected virtual void OnLeftRelease() => ForwardMessageToHits("OnLeftRelease");
        protected virtual void OnRightPress() => ForwardMessageToHits("OnRightPress");
        protected virtual void OnRightRelease() => ForwardMessageToHits("OnRightRelease");
        protected virtual void OnMiddlePress() => ForwardMessageToHits("OnMiddlePress");
        protected virtual void OnMiddleRelease() => ForwardMessageToHits("OnMiddleRelease");
        protected virtual void OnScroll(InputValue value) => ForwardMessageToHits("OnScroll", value.Get<float>());
        protected virtual void OnPointer(InputValue value)
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

        protected virtual void OnMove(InputValue value) => ForwardMessageToListeners("OnMove", value.Get<Vector2>());
        protected virtual void OnAct() => ForwardMessageToListeners("OnAct");
        protected virtual void OnCrouch() => ForwardMessageToListeners("OnCrouch");
        protected virtual void OnJump(InputValue value) => ForwardMessageToListeners("OnJump", value.Get<float>());
        protected virtual void OnRetry() => ForwardMessageToListeners("OnRetry");

        protected virtual void OnSave() => ForwardMessageToHits("OnSave");
        protected virtual void OnLoad() => ForwardMessageToHits("OnLoad");
        protected virtual void OnDebugTest() => debugLogic?.Invoke();
        protected virtual void OnEscape()
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
