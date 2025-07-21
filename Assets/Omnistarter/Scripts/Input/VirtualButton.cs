using Omnis;
using UnityEngine.Events;

public class VirtualButton : PointerBase
{
    public UnityEvent callback;

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
}
