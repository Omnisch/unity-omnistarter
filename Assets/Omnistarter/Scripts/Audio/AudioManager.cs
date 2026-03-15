// author: Omnistudio
// version: 2026.03.15

using UnityEngine;
using UnityEngine.Audio;

namespace Omnis.Audio
{
    public sealed partial class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioMixer mixer;

        [SerializeField] private AudioMixerGroup musicStateGroup;
        [SerializeField] private AudioMixerGroup sfxStateGroup;
        [SerializeField] private AudioMixerGroup uiStateGroup;
        [SerializeField] private AudioMixerGroup ambienceStateGroup;
        
        [SerializeField] private AudioMixerSnapshot defaultSnapshot;
        [SerializeField] private AudioMixerSnapshot pausedSnapshot;
        
        #region Unity Messages
        private void Awake() {
            if (!EnsureSingleton())
                return;
        }
        #endregion
    }
}
