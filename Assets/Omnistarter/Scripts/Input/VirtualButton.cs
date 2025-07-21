namespace Omnis
{
    /// <summary>
    /// Call <i>callback</i> when left button clicked on self.
    /// </summary>
    public class VirtualButton : PointerBase
    {
        public UnityEngine.Events.UnityEvent callback;

        public override bool LeftPressed
        {
            get => base.LeftPressed;

            protected set
            {
                base.LeftPressed = value;

                if (!value && Pointed)
                    callback?.Invoke();
            }
        }

        public override bool Pointed
        {
            get => base.Pointed;
            protected set
            {
                base.Pointed = value;

                if (!value)
                    LeftPressed = false;
            }
        }
    }
}
