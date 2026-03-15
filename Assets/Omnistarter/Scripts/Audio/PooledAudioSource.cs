// author: Omnistudio
// version: 2026.03.15

using System;
using System.Collections;
using Omnis.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace Omnis.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public sealed class PooledAudioSource : MonoBehaviour
    {
        public bool IsBusy { get; private set; }
        public bool IsLooping { get; private set; }
        public AudioBus CurrentBus { get; private set; }
        public int CurrentPriority { get; private set; }
        public float StartedAtRealtime { get; private set; }

        private AudioSource source;
        private Transform followTarget;
        private Action<PooledAudioSource> onReleased;

        private Coroutine releaseRoutine;
        private Coroutine fadeRoutine;

        private float baseVolume = 1f;

        private void Awake() {
            source = GetComponent<AudioSource>();
            source.playOnAwake = false;
            source.loop = false;
            source.spatialBlend = 0f;
        }

        private void LateUpdate() {
            if (followTarget != null) {
                transform.position = followTarget.position;
            }
        }

        public void Play(
            AudioCue cue,
            AudioClip clip,
            AudioMixerGroup mixerGroup,
            in AudioPlayRequest request,
            Action<PooledAudioSource> onReleasedCallback) {
            if (cue == null || clip == null)
                return;

            StopImmediately(notifyRelease: false);

            onReleased = onReleasedCallback;
            followTarget = request.FollowTarget;

            IsBusy = true;
            IsLooping = cue.loop;
            CurrentBus = cue.bus;
            CurrentPriority = cue.priority;
            StartedAtRealtime = Time.unscaledTime;

            ConfigureSource(cue, clip, mixerGroup, request);

            transform.position = followTarget != null
                ? followTarget.position
                : request.Position;

            source.Play();

            if (!cue.loop) {
                releaseRoutine = StartCoroutine(ReleaseWhenFinishedCoroutine());
            }
        }

        public void Stop(float fadeSeconds = 0f) {
            if (!IsBusy)
                return;

            if (fadeSeconds <= 0f) {
                StopImmediately(true);
                return;
            }

            if (fadeRoutine != null) {
                StopCoroutine(fadeRoutine);
            }

            fadeRoutine = StartCoroutine(FadeOutAndReleaseCoroutine(fadeSeconds));
        }

        public void SetFollowTarget(Transform target) {
            followTarget = target;
        }

        public void SetLocalVolume(float volume) {
            if (!IsBusy)
                return;

            source.volume = Mathf.Clamp01(volume);
        }

        private void ConfigureSource(
            AudioCue cue,
            AudioClip clip,
            AudioMixerGroup mixerGroup,
            in AudioPlayRequest request) {
            source.outputAudioMixerGroup = mixerGroup;
            source.clip = clip;
            source.loop = cue.loop;
            source.priority = Mathf.Clamp(cue.priority, 0, 255);

            bool is2D = request.Override2D ? request.Is2DOverride : cue.is2D;
            source.spatialBlend = is2D ? 0f : Mathf.Clamp01(cue.spatialBlend);
            source.minDistance = cue.minDistance;
            source.maxDistance = cue.maxDistance;

            float randomVolume = UnityEngine.Random.Range(cue.volumeRange.x, cue.volumeRange.y);
            float randomPitch = UnityEngine.Random.Range(cue.pitchRange.x, cue.pitchRange.y);

            baseVolume = Mathf.Clamp01(randomVolume * request.VolumeMultiplier);
            source.volume = baseVolume;
            source.pitch = Mathf.Max(0.01f, randomPitch * request.PitchMultiplier);
        }

        private IEnumerator ReleaseWhenFinishedCoroutine() {
            // Wait until the clip finishes naturally.
            while (source != null && source.isPlaying) {
                yield return null;
            }

            Release();
        }

        private IEnumerator FadeOutAndReleaseCoroutine(float fadeSeconds) {
            float from = source.volume;

            yield return YieldHelper.EEase(
                Easing.Linear,
                value => {
                    if (source != null)
                        source.volume = Mathf.Lerp(from, 0f, value);
                },
                () => StopImmediately(true),
                fadeSeconds
            );
        }

        private void StopImmediately(bool notifyRelease) {
            if (releaseRoutine != null) {
                StopCoroutine(releaseRoutine);
                releaseRoutine = null;
            }

            if (fadeRoutine != null) {
                StopCoroutine(fadeRoutine);
                fadeRoutine = null;
            }

            if (source != null) {
                source.Stop();
                source.clip = null;
                source.volume = 0f;
                source.pitch = 1f;
            }

            followTarget = null;
            IsBusy = false;
            IsLooping = false;
            CurrentPriority = 255;
            StartedAtRealtime = 0f;
            baseVolume = 1f;

            if (notifyRelease) {
                Release();
            }
        }

        private void Release() {
            if (!IsBusy) {
                // Release can be called after natural finish, so make sure we still reset the source.
                if (source != null) {
                    source.Stop();
                    source.clip = null;
                    source.volume = 0f;
                    source.pitch = 1f;
                }
            }

            followTarget = null;
            IsBusy = false;
            IsLooping = false;
            CurrentPriority = 255;
            StartedAtRealtime = 0f;
            baseVolume = 1f;

            var callback = onReleased;
            onReleased = null;
            callback?.Invoke(this);
        }
    }
}
