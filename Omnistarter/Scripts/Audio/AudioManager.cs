// author: Omnistudio
// version: 2024.12.15

using UnityEngine;

namespace Omnis
{
    [RequireComponent(typeof(AudioSource))]
    public partial class AudioManager : MonoBehaviour
    {
        #region Fields
        private AudioSource source;
        private bool mute;
        #endregion

        #region Interfaces
        public bool Mute
        {
            get => mute;
            set => mute = value;
        }
        public void PlaySE(string seName)
        {
            try
            {
                PlaySE(System.Enum.Parse<SoundEffectName>(seName));
            }
            catch
            {
                Debug.LogWarning($"No sound effect named {seName}.");
            }
        }
        public void PlaySE(SoundEffectName seName)
        {
            if (!Mute)
                source.PlayOneShot(GameManager.Instance.SeSettings.soundEffects.Find(se => se.name == seName).se);
        }
        #endregion

        #region Unity Methods
        private void Start()
        {
            source = GetComponent<AudioSource>();
        }
        #endregion
    }
}
