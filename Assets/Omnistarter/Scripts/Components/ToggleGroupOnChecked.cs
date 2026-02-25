// author: ChatGPT
// version: 2026.02.25

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Omnis
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ToggleGroup))]
    public class ToggleGroupOnChecked : MonoBehaviour
    {
        [Serializable] public class IntEvent : UnityEvent<int> { }
        [Serializable] public class StringEvent : UnityEvent<string> { }
        [Serializable] public class ToggleEvent : UnityEvent<Toggle> { }

        [Header("Raised when a toggle becomes ON")]
        public IntEvent OnCheckedId = new();
        public StringEvent OnCheckedName = new();
        public ToggleEvent OnCheckedToggle = new();

        [Tooltip("If true, fires once at Start for the initially ON toggle (if any).")]
        public bool FireInitial = true;

        private ToggleGroup _group;
        private readonly List<Toggle> _toggles = new();

        private void Awake() {
            _group = GetComponent<ToggleGroup>();

            // Collect toggles under this object (you can switch to true if nested).
            GetComponentsInChildren(false, _toggles);

            // Ensure they belong to this group and register callbacks.
            foreach (var t in _toggles) {
                if (t == null) continue;
                t.group = _group;

                // Capture local variable to avoid closure pitfalls.
                var toggle = t;
                toggle.onValueChanged.AddListener(isOn =>
                {
                    if (!isOn) return; // only care when turned ON

                    int id = toggle.transform.GetSiblingIndex();
                    OnCheckedId.Invoke(id);
                    string name = toggle.gameObject.name;
                    OnCheckedName.Invoke(name);
                    OnCheckedToggle.Invoke(toggle);
                });
            }
        }

        private void Start() {
            if (!FireInitial) return;

            // If one is already ON, emit it once.
            foreach (var t in _toggles) {
                if (t != null && t.isOn) {
                    int id = t.transform.GetSiblingIndex();
                    OnCheckedId.Invoke(id);
                    string name = t.gameObject.name;
                    OnCheckedName.Invoke(name);
                    OnCheckedToggle.Invoke(t);
                    break;
                }
            }
        }
    }
}
