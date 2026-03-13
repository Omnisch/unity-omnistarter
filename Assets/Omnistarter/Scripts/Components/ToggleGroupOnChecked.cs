// author: ChatGPT
// version: 2026.03.13

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
        public IntEvent onCheckedId = new();
        public StringEvent onCheckedName = new();
        public ToggleEvent onCheckedToggle = new();

        [Tooltip("If true, fires once at Start for the initially ON toggle (if any).")]
        public bool fireInitial = true;

        private ToggleGroup group;
        private readonly List<Toggle> toggles = new();

        private void Awake() {
            group = GetComponent<ToggleGroup>();

            // Collect toggles under this object (you can switch to true if nested).
            GetComponentsInChildren(false, toggles);

            // Ensure they belong to this group and register callbacks.
            foreach (var t in toggles) {
                if (t == null) continue;
                t.group = group;

                // Capture local variable to avoid closure pitfalls.
                var toggle = t;
                toggle.onValueChanged.AddListener(isOn =>
                {
                    if (!isOn) return; // only care when turned ON

                    int id = toggle.transform.GetSiblingIndex();
                    onCheckedId.Invoke(id);
                    string n = toggle.gameObject.name;
                    onCheckedName.Invoke(n);
                    onCheckedToggle.Invoke(toggle);
                });
            }
        }

        private void Start() {
            if (!fireInitial) return;

            // If one is already ON, emit it once.
            foreach (var t in toggles) {
                if (t != null && t.isOn) {
                    int id = t.transform.GetSiblingIndex();
                    onCheckedId.Invoke(id);
                    string n = t.gameObject.name;
                    onCheckedName.Invoke(n);
                    onCheckedToggle.Invoke(t);
                    break;
                }
            }
        }
    }
}
