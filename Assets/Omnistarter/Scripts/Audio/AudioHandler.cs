// author: Omnistudio
// version: 2025.03.30

using UnityEngine;

namespace Omnis.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public partial class AudioHandler : MonoBehaviour
    {
        #region Serialized Fields
        [SerializeField] private AudioSettings audioSettings;
        #endregion

        #region Fields
        private AudioSource source;
        private bool mute;
        #endregion

        #region Interfaces
        public bool Mute
        {
            get => mute;
            set
            {
                mute = value;
                if (value) source.Stop();
            }
        }
        public void PlaySE(string seName)
        {
            if (!Mute)
                source.PlayOneShot(audioSettings.soundEffects.Find(se => se.name == seName).audio);
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
