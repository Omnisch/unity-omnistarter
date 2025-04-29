// author: Omnistudio
// version: 2024.10.28

using UnityEngine;

namespace Omnis
{
    /// <summary>
    /// When Invoke() is called, delay for <i>delayTime</i> seconds first.
    /// </summary>
    public class DelayedLogic : Logic
    {
        [Tooltip("In seconds.")]
        [UnityEngine.Min(0)] public float delayTime;

        public override void Invoke() => StartCoroutine(InvokingCoroutine());

        private System.Collections.IEnumerator InvokingCoroutine()
        {
            yield return new UnityEngine.WaitForSecondsRealtime(delayTime);
            callback.Invoke();
        }
    }
}
