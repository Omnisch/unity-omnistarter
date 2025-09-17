// author: Omnistudio
// version: 2025.09.17

namespace Omnis
{
    /// <summary>
    /// Call <i>onCallback</i> and <i>offCallback</i> in turn when left button clicked on self.
    /// </summary>
    public class VirtualSwitch : PointerBase
    {
        public bool state = false;
        public UnityEngine.Events.UnityEvent<bool> onCallback;
        public UnityEngine.Events.UnityEvent<bool> offCallback;

        private bool canceled = false;

        public override bool LeftPressed {
            get => base.LeftPressed;

            protected set {
                base.LeftPressed = value;

                if (value) {
                    canceled = false;
                } else {
                    if (!canceled) {
                        state = !state;
                        if (state)
                            onCallback?.Invoke(true);
                        else
                            offCallback?.Invoke(false);
                    }
                }
            }
        }

        public override bool Pointed {
            get => base.Pointed;
            protected set {
                base.Pointed = value;

                if (!value && LeftPressed)
                    canceled = true;
            }
        }

        private void CancelClick() {
            canceled = true;
        }
    }
}
