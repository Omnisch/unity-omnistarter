// author: Omnistudio
// version: 2026.03.13

using Omnis.Utils;
using OmnisEditor;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace Omnis
{
    /// <summary>
    /// Call <i>callback</i> when left button clicked on self.
    /// </summary>
    public class VirtualButton : PointerBase
    {
        public UnityEvent enterCallback;
        public UnityEvent pressCallback;
        public UnityEvent releaseCallback;
        public UnityEvent exitCallback;
        [ConditionalGroup]
        public LongPress longPress;
        [Header("Animation")]
        public float animScale = 1.1f;
        public float easingTime = 0.6f;
        [ConditionalGroup]
        public EasingSettings easing = new(Easing.EasingType.ElasticOut);

        private bool canceled;
        private float longPressProgress; // 0 ~ 1
        private Coroutine longPressCoroutine;
        private Coroutine zoomCoroutine;
        private Coroutine ZoomCoroutine {
            set {
                if (zoomCoroutine != null)
                    StopCoroutine(zoomCoroutine);
                zoomCoroutine = value;
            }
        }
        private Vector3 originalLocalScale;


        public override bool LeftPressed {
            get => base.LeftPressed;
            protected set {
                base.LeftPressed = value;

                if (value) {
                    pressCallback?.Invoke();
                    canceled = false;

                    // Long press
                    if (longPress.needLongPress) {
                        longPressCoroutine = LongPressHold();
                    }

                    // animation
                    ZoomCoroutine = Zoom(animScale == 0f ? 1f : 1f / animScale);

                } else {
                    if (!canceled) {
                        releaseCallback?.Invoke();
                        AbortLongPress();

                        // animation
                        ZoomCoroutine = Zoom(animScale);
                    }
                }
            }
        }

        public override bool Pointed {
            get => base.Pointed;
            protected set {
                base.Pointed = value;

                if (value) {
                    enterCallback?.Invoke();

                    // animation
                    ZoomCoroutine = Zoom(animScale);

                } else {
                    exitCallback?.Invoke();

                    if (LeftPressed) {
                        canceled = true;
                    }

                    // animation
                    ZoomCoroutine = Zoom(1f);
                }

                // Whether this is pointed or not, both should abort potential long press.
                AbortLongPress();
            }
        }

        private void AbortLongPress() {
            if (longPress.needLongPress) {
                if (longPressCoroutine != null) {
                    StopCoroutine(longPressCoroutine);
                }
                if (longPressProgress is > 0f and < 1f) {
                    longPress.notLongEnoughCallback?.Invoke();
                }
            }
        }


        private Coroutine LongPressHold() {
            return this.Ease(
                value => {
                    longPressProgress = value;
                    longPress.progressCallback?.Invoke(value);
                },
                () => {
                    longPress.longPressCallback?.Invoke();
                    longPressProgress = 0f;
                },
                Easing.Linear,
                longPress.pressTime
            );
        }
        private Coroutine Zoom(float scale) {
            if (!isActiveAndEnabled) return null;

            var oldLocalScale = transform.localScale;
            var newLocalScale = scale * originalLocalScale;

            if (oldLocalScale == newLocalScale) return null;

            return this.Ease(
                value => {
                    transform.localScale = Vector3.Lerp(oldLocalScale, newLocalScale, value);
                },
                easing.Evaluate, easingTime
            );
        }

        private void Start() {
            originalLocalScale = transform.localScale;
        }


        #region Structs
        [Serializable]
        public class LongPress
        {
            public bool needLongPress;
            [ShowIf("needLongPress", true)]
            public float pressTime = 1f;
            [ShowIf("needLongPress", true)]
            public UnityEvent<float> progressCallback;
            [ShowIf("needLongPress", true)]
            public UnityEvent longPressCallback;
            [ShowIf("needLongPress", true)]
            public UnityEvent notLongEnoughCallback;
        }
        #endregion
    }
}
