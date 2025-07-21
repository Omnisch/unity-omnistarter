// author: Omnistudio
// version: 2025.07.21

using UnityEngine.Events;

namespace Omnis
{
    public class ClickLogic : PointerBase
    {
        #region Serialized fields
        public UnityEvent enterCallback;
        public UnityEvent pressCallback;
        public UnityEvent releaseCallback;
        public UnityEvent exitCallback;
        #endregion

        #region Properties
        public override bool Pointed
        {
            get => base.Pointed;
            protected set
            {
                base.Pointed = value;
                if (value) enterCallback?.Invoke();
                else exitCallback?.Invoke();
            }
        }
        public override bool LeftPressed
        {
            get => base.LeftPressed;
            protected set
            {
                base.LeftPressed = value;
                if (value) pressCallback?.Invoke();
                else releaseCallback?.Invoke();
            }
        }
        #endregion
    }
}
