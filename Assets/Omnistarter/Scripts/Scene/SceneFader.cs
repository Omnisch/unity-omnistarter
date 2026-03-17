// author: Omnistudio
// version: 2026.03.18

using System.Collections;
using Omnis.Audio;
using Omnis.Utils;
using OmnisEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace Omnis.SceneManagement
{
    public class SceneFader : SceneTransitionBase
    {
        [SerializeField] private GameObject curtainPrefab;
        [SerializeField] private AudioMixerSnapshot backgroundSnapshot;
        [SerializeField] private AudioMixerSnapshot defaultSnapshot;
        [Header("Animation")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] [ConditionalGroup] private EasingSettings easing;
        
        private GameObject curtain;
        
        public override IEnumerator TransitionOut(string sceneName) {
            if (curtain != null) {
                Destroy(curtain);
            }
            
            curtain = Instantiate(curtainPrefab);
            DontDestroyOnLoad(curtain);

            curtain.GetComponent<Canvas>().worldCamera = Camera.main;
            yield return YieldHelper.EEase(
                easing.Evaluate,
                value => {
                    curtain.GetComponent<CanvasGroup>().alpha = value;
                    AudioManager.Instance.TransitionToSnapshot(backgroundSnapshot, fadeDuration);
                }, time: fadeDuration);
        }
        
        public override IEnumerator TransitionIn(string sceneName) {
            if (curtain == null) {
                yield break;
            }
            
            curtain.GetComponent<Canvas>().worldCamera = Camera.main;
            yield return YieldHelper.EEase(
                easing.Evaluate,
                value => {
                    curtain.GetComponent<CanvasGroup>().alpha = value.Inv();
                    AudioManager.Instance.TransitionToSnapshot(defaultSnapshot, fadeDuration);
                }, time: fadeDuration);

            Destroy(curtain);
        }
    }
}
