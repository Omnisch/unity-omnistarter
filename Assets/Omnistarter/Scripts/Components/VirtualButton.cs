// author: Omnistudio
// version: 2026.03.08

using Omnis.Editor;
using Omnis.Utils;
using System;
using System.Collections;
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
        [ConditionalGroup]
        public Animation anim;

        private bool canceled = false;
        private float longPressProgress = 0; // 0 ~ 1
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
                        longPressCoroutine = StartCoroutine(LongPressHold());
                    }

                    // animation
                    if (anim.animType == Animation.AnimType.Zooming) {
                        ZoomCoroutine = StartCoroutine(Zoom(anim.scale == 0f ? 1f : (1f / anim.scale)));
                    }

                } else {
                    if (!canceled) {
                        releaseCallback?.Invoke();
                        AbortLongPress();

                        // animation
                        if (anim.animType == Animation.AnimType.Zooming) {
                            ZoomCoroutine = StartCoroutine(Zoom(anim.scale));
                        }
                    } else {
                        // animation
                        if (anim.animType == Animation.AnimType.Zooming) {
                            ZoomCoroutine = StartCoroutine(Zoom(1f));
                        }
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
                    if (anim.animType == Animation.AnimType.Zooming) {
                        ZoomCoroutine = StartCoroutine(Zoom(anim.scale));
                    }

                } else {
                    exitCallback?.Invoke();

                    if (LeftPressed) {
                        canceled = true;
                    }

                    // animation
                    if (anim.animType == Animation.AnimType.Zooming) {
                        ZoomCoroutine = StartCoroutine(Zoom(1f));
                    }
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
                if (longPressProgress > 0f && longPressProgress < 1f) {
                    longPress.notLongEnoughCallback?.Invoke();
                }
            }
        }


        private IEnumerator LongPressHold() {
            yield return YieldHelper.Ease(
                (value) => {
                    longPressProgress = value;
                    longPress.progressCallback?.Invoke(value);
                },
                () => {
                    longPress.longPressCallback?.Invoke();
                    longPressProgress = 0f;
                },
                Easing.Linear,
                longPress.pressTime);
        }
        private IEnumerator Zoom(float scale) {
            var oldLocalScale = transform.localScale;
            var newLocalScale = scale * originalLocalScale;
            Func<float, float> easingFunc = anim.easeType switch {
                Animation.ZoomingEaseType.Bounce => Easing.OutBounce,
                Animation.ZoomingEaseType.Elastic => Easing.OutElastic,
                _ => Easing.OutQuart
            };
            float time = anim.easeType switch {
                Animation.ZoomingEaseType.Bounce => 0.2f,
                Animation.ZoomingEaseType.Elastic => 0.6f,
                _ => 0.1f
            };

            yield return YieldHelper.Ease(
                (value) => {
                    transform.localScale = Vector3.Lerp(oldLocalScale, newLocalScale, value);
                },
                easingFunc, time);
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

        [Serializable]
        public class Animation
        {
            public AnimType animType = AnimType.None;
            [ShowIf("animType", AnimType.Zooming)]
            public float scale = 1.1f;
            [ShowIf("animType", AnimType.Zooming)]
            public ZoomingEaseType easeType = ZoomingEaseType.Elastic;

            public enum AnimType { None, Zooming }
            public enum ZoomingEaseType { Bounce, Elastic, Smoothstep }
        }
        #endregion
    }
}
