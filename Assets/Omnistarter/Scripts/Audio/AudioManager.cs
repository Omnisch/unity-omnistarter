// author: Omnistudio
// version: 2026.03.18

using UnityEngine;
using UnityEngine.Audio;

namespace Omnis.Audio
{
    public sealed class AudioManager : MonoBehaviour
    {
        [Header("Subsystems")]
        [SerializeField] private MusicPlayer musicPlayer;
        [SerializeField] private SfxPlayer sfxPlayer;

        [Header("Mixer")]
        [SerializeField] private AudioMixer mixer;
        [SerializeField] private AudioMixerSnapshot defaultSnapshot;
        [SerializeField] private AudioMixerSnapshot pausedSnapshot;

        [Header("Exposed Parameters")]
        [SerializeField] private string masterVolumeParam = "MasterUserVolume";
        [SerializeField] private string musicVolumeParam = "MusicUserVolume";
        [SerializeField] private string sfxVolumeParam = "SfxUserVolume";
        [SerializeField] private string uiVolumeParam = "UiUserVolume";

        [Header("Runtime Values")]
        [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;
        [SerializeField, Range(0f, 1f)] private float uiVolume = 1f;

        public float MasterVolume => masterVolume;
        public float MusicVolume => musicVolume;
        public float SfxVolume => sfxVolume;
        public float UiVolume => uiVolume;

        public static AudioManager Instance;

        private const string MasterVolumeKey = "Audio.MasterVolume";
        private const string MusicVolumeKey = "Audio.MusicVolume";
        private const string SfxVolumeKey = "Audio.SfxVolume";
        private const string UiVolumeKey = "Audio.UiVolume";

        private void Awake() {
            Instance = this;

            LoadVolumes();
            ApplyAllVolumes();
            ApplyDefaultSnapshotImmediate();
        }

        private void OnDestroy() {
            Instance = null;
        }


        #region SFX

        public PooledAudioSource PlaySfx(AudioCue cue, in AudioPlayRequest request) {
            if (sfxPlayer == null || cue == null)
                return null;

            return sfxPlayer.Play(cue, request);
        }

        public PooledAudioSource PlaySfx(AudioCue cue, Vector3 position) {
            return PlaySfx(cue, new AudioPlayRequest(position));
        }

        public PooledAudioSource PlayUi(AudioCue cue, float volumeMultiplier = 1f) {
            if (cue == null)
                return null;

            return PlaySfx(cue, new AudioPlayRequest(
                position: Vector3.zero,
                followTarget: null,
                volumeMultiplier: volumeMultiplier,
                pitchMultiplier: 1f,
                override2D: true,
                is2DOverride: true));
        }

        public PooledAudioSource PlayLoop(AudioCue cue, Transform followTarget = null, float volumeMultiplier = 1f) {
            if (cue == null)
                return null;

            Vector3 pos = followTarget != null ? followTarget.position : Vector3.zero;

            return PlaySfx(cue, new AudioPlayRequest(
                position: pos,
                followTarget: followTarget,
                volumeMultiplier: volumeMultiplier));
        }

        public void StopAllSfx(float fadeSeconds = 0f) {
            if (sfxPlayer == null)
                return;

            sfxPlayer.StopAll(fadeSeconds);
        }

        public void StopAllSfxOnBus(AudioBus bus, float fadeSeconds = 0f) {
            if (sfxPlayer == null)
                return;

            sfxPlayer.StopAllOnBus(bus, fadeSeconds);
        }

        #endregion


        #region Music

        public void PlayMusic(MusicCue cue) {
            if (musicPlayer == null || cue == null)
                return;

            musicPlayer.Play(cue);
        }

        public void TransitionMusic(in MusicTransitionRequest request) {
            if (musicPlayer == null || request.cue == null)
                return;

            musicPlayer.Transition(request);
        }

        public void SetMusicVariant(int variantIndex, float fadeSeconds = 0.5f) {
            if (musicPlayer == null)
                return;

            musicPlayer.SetVariant(variantIndex, fadeSeconds);
        }

        public void StopMusic(float fadeSeconds = 1f) {
            if (musicPlayer == null)
                return;

            musicPlayer.StopMusic(fadeSeconds);
        }

        #endregion


        #region Volume

        public void SetMasterVolume(float value01) {
            masterVolume = Mathf.Clamp01(value01);
            SetMixerVolume(masterVolumeParam, masterVolume);
            SaveVolumes();
        }

        public void SetMusicVolume(float value01) {
            musicVolume = Mathf.Clamp01(value01);
            SetMixerVolume(musicVolumeParam, musicVolume);
            SaveVolumes();
        }

        public void SetSfxVolume(float value01) {
            sfxVolume = Mathf.Clamp01(value01);
            SetMixerVolume(sfxVolumeParam, sfxVolume);
            SaveVolumes();
        }

        public void SetUiVolume(float value01) {
            uiVolume = Mathf.Clamp01(value01);
            SetMixerVolume(uiVolumeParam, uiVolume);
            SaveVolumes();
        }

        public float GetBusVolume(AudioBus bus) {
            return bus switch {
                AudioBus.Music => musicVolume,
                AudioBus.Ui => uiVolume,
                _ => sfxVolume
            };
        }

        public void SetBusVolume(AudioBus bus, float value01) {
            switch (bus) {
                case AudioBus.Music:
                    SetMusicVolume(value01);
                    break;
                case AudioBus.Ui:
                    SetUiVolume(value01);
                    break;
                case AudioBus.Sfx:
                case AudioBus.Ambience:
                    SetSfxVolume(value01);
                    break;
            }
        }

        private void ApplyAllVolumes() {
            SetMixerVolume(masterVolumeParam, masterVolume);
            SetMixerVolume(musicVolumeParam, musicVolume);
            SetMixerVolume(sfxVolumeParam, sfxVolume);
            SetMixerVolume(uiVolumeParam, uiVolume);
        }

        private void SetMixerVolume(string paramName, float value01) {
            if (mixer == null || string.IsNullOrWhiteSpace(paramName))
                return;

            mixer.SetFloat(paramName, LinearToDb(value01));
        }

        private static float LinearToDb(float value01) {
            if (value01 <= 0.0001f)
                return -80f;

            return Mathf.Log10(value01) * 20f;
        }

        #endregion


        #region Snapshots

        public void SetPaused(bool paused, float transitionSeconds = 0.15f) {
            TransitionToSnapshot(paused ? pausedSnapshot : defaultSnapshot, transitionSeconds);
        }

        public void TransitionToSnapshot(AudioMixerSnapshot snapshot, float transitionSeconds = 0.15f) {
            if (snapshot == null)
                return;

            snapshot.TransitionTo(Mathf.Max(0f, transitionSeconds));
        }

        private void ApplyDefaultSnapshotImmediate() {
            if (defaultSnapshot != null) {
                defaultSnapshot.TransitionTo(0f);
            }
        }

        #endregion


        #region Save / Load

        private void LoadVolumes() {
            masterVolume = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
            musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
            sfxVolume = PlayerPrefs.GetFloat(SfxVolumeKey, 1f);
            uiVolume = PlayerPrefs.GetFloat(UiVolumeKey, 1f);
        }

        private void SaveVolumes() {
            PlayerPrefs.SetFloat(MasterVolumeKey, masterVolume);
            PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
            PlayerPrefs.SetFloat(SfxVolumeKey, sfxVolume);
            PlayerPrefs.SetFloat(UiVolumeKey, uiVolume);
            PlayerPrefs.Save();
        }

        #endregion
    }
}
