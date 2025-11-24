// author: Omnistudio
// version: 2025.11.24

using Omnis.Editor;
using Omnis.Utils;
using System;
using UnityEngine.Events;

namespace Omnis
{
    /// <summary>
    /// Call <i>callback</i> when left button clicked on self.
    /// </summary>
    public class VirtualButton : PointerBase
    {
        [ConditionalGroup]
        public LongPress longPress;
        public UnityEvent enterCallback;
        public UnityEvent pressCallback;
        public UnityEvent releaseCallback;
        public UnityEvent exitCallback;

        private bool canceled = false;
        private float longPressProgress = 0; // 0 ~ 1


        public override bool LeftPressed {
            get => base.LeftPressed;
            protected set {
                base.LeftPressed = value;

                if (value) {
                    pressCallback?.Invoke();
                    canceled = false;

                    // Long press
                    if (longPress.needLongPress) {
                        StartCoroutine(YieldHelper.Ease(
                            (value) => {
                                longPressProgress = value;
                                longPress.progressCallback?.Invoke(value);
                                if (value == 1f) {
                                    longPress.longPressCallback?.Invoke();
                                }
                            },
                            Easing.Linear,
                            time: longPress.pressTime
                        ));
                    }

                } else {
                    if (!canceled) {
                        releaseCallback?.Invoke();
                        AbortLongPress();
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
                } else {
                    exitCallback?.Invoke();
                    if (LeftPressed) {
                        canceled = true;
                        AbortLongPress();
                    }
                }

                // Stop long press progress
                StopAllCoroutines();
            }
        }

        private void AbortLongPress() {
            if (longPress.needLongPress) {
                StopAllCoroutines();
                if (longPressProgress > 0f && longPressProgress < 1f) {
                    longPress.notLongEnoughCallback?.Invoke();
                }
            }
        }


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
    }
}
