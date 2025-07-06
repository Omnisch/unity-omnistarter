// author: WangNianyi2001
// version: 2025.07.06

using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// When Invoke() is called, delay for <i>delayTime</i> seconds first.
    /// </summary>
    public class DelayedLogic : Logic
    {
        [Tooltip("In seconds.")]
        [Min(0)] public float delayTime;

        public override void Invoke() => StartCoroutine(InvokingCoroutine());

        private System.Collections.IEnumerator InvokingCoroutine()
        {
            yield return new WaitForSecondsRealtime(delayTime);
            callback.Invoke();
        }
    }
}
