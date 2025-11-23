// author: Omnistudio
// version: 2025.11.23

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

        private bool canceled = false;

        public override bool LeftPressed
        {
            get => base.LeftPressed;

            protected set
            {
                base.LeftPressed = value;

                if (value) {
                    pressCallback?.Invoke();
                    canceled = false;
                }
                else {
                    if (!canceled) {
                        releaseCallback?.Invoke();
                    }
                }
            }
        }

        public override bool Pointed
        {
            get => base.Pointed;
            protected set
            {
                base.Pointed = value;

                if (value) {
                    enterCallback?.Invoke();
                } else {
                    exitCallback?.Invoke();
                    if (LeftPressed) {
                        canceled = true;
                    }
                }
            }
        }

        private void CancelClick()
        {
            canceled = true;
        }
    }
}
