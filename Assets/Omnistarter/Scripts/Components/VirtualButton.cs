// author: Omnistudio
// version: 2025.09.17

namespace Omnis
{
    /// <summary>
    /// Call <i>callback</i> when left button clicked on self.
    /// </summary>
    public class VirtualButton : PointerBase
    {
        public UnityEngine.Events.UnityEvent callback;

        private bool canceled = false;

        public override bool LeftPressed
        {
            get => base.LeftPressed;

            protected set
            {
                base.LeftPressed = value;

                if (value) {
                    this.canceled = false;
                }
                else {
                    if (!this.canceled)
                        this.callback?.Invoke();
                }
            }
        }

        public override bool Pointed
        {
            get => base.Pointed;
            protected set
            {
                base.Pointed = value;

                if (!value && this.LeftPressed)
                    this.canceled = true;
            }
        }

        private void CancelClick()
        {
            this.canceled = true;
        }
    }
}
