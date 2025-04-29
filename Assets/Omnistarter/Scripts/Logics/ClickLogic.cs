// author: Omnistudio
// version: 2024.12.04

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
            set
            {
                base.Pointed = value;
                if (value) enterCallback?.Invoke();
                else exitCallback?.Invoke();
            }
        }
        public override bool LeftPressed
        {
            get => base.LeftPressed;
            set
            {
                base.LeftPressed = value;
                if (value) pressCallback?.Invoke();
                else releaseCallback?.Invoke();
            }
        }
        #endregion
    }
}
