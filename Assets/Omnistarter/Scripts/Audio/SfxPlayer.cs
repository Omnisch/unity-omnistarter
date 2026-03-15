// author: Omnistudio
// version: 2026.03.15

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Omnis.Audio
{
    public sealed class SfxPlayer : MonoBehaviour
    {
        [Header("Pool")]
        [SerializeField] private PooledAudioSource sourcePrefab;
        [SerializeField] private int initialPoolSize = 8;
        [SerializeField] private int maxPoolSize = 32;

        [Header("Mixer Groups")]
        [SerializeField] private AudioMixerGroup sfxMixerGroup;
        [SerializeField] private AudioMixerGroup uiMixerGroup;
        [SerializeField] private AudioMixerGroup ambienceMixerGroup;

        private Transform poolRoot;
        private readonly List<PooledAudioSource> sources = new();
        private readonly Dictionary<AudioCue, CueRuntimeState> cueStates = new();
        private readonly Dictionary<PooledAudioSource, AudioCue> sourceOwners = new();

        private void Awake() {
            poolRoot = new GameObject("SfxPoolRoot").transform;
            poolRoot.SetParent(transform, false);

            for (int i = 0; i < initialPoolSize; i++) {
                CreateSource();
            }
        }

        public PooledAudioSource Play(AudioCue cue, in AudioPlayRequest request) {
            if (!CanPlayCue(cue, out CueRuntimeState state))
                return null;

            int clipIndex = PickClipIndex(cue, state);
            if (clipIndex < 0 || clipIndex >= cue.clips.Length)
                return null;

            AudioClip clip = cue.clips[clipIndex];
            if (clip == null)
                return null;

            PooledAudioSource source = AcquireSource(cue);
            if (source == null)
                return null;

            AudioMixerGroup mixerGroup = ResolveMixerGroup(cue.bus);

            state.lastPlayTime = Time.unscaledTime;
            state.activeVoices++;

            source.Play(
                cue,
                clip,
                mixerGroup,
                request,
                OnSourceReleased);

            sourceOwners[source] = cue;
            return source;
        }

        public void StopAllOnBus(AudioBus bus, float fadeSeconds = 0f) {
            foreach (var src in sources.Where(src => src.IsBusy && src.CurrentBus == bus)) {
                src.Stop(fadeSeconds);
            }
        }

        public void StopAll(float fadeSeconds = 0f) {
            foreach (var src in sources.Where(src => src.IsBusy)) {
                src.Stop(fadeSeconds);
            }
        }

        private bool CanPlayCue(AudioCue cue, out CueRuntimeState state) {
            state = null;

            if (cue == null || cue.clips == null || cue.clips.Length == 0)
                return false;

            if (!cueStates.TryGetValue(cue, out state)) {
                state = new CueRuntimeState();
                cueStates.Add(cue, state);
            }

            if (cue.cooldownSeconds > 0f) {
                float now = Time.unscaledTime;
                if (now - state.lastPlayTime < cue.cooldownSeconds)
                    return false;
            }

            if (cue.maxSimultaneousVoices > 0 && state.activeVoices >= cue.maxSimultaneousVoices)
                return false;

            return true;
        }

        private int PickClipIndex(AudioCue cue, CueRuntimeState state) {
            if (cue.clips == null || cue.clips.Length == 0)
                return -1;

            if (cue.clips.Length == 1) {
                state.lastIndex = 0;
                return 0;
            }

            switch (cue.pickMode) {
                case ClipPickMode.Sequential:
                {
                    int next = (state.lastIndex + 1) % cue.clips.Length;
                    state.lastIndex = next;
                    return next;
                }

                case ClipPickMode.ShuffleNoImmediateRepeat:
                {
                    EnsureShuffleBag(cue, state);

                    int next = state.shuffleBag.Dequeue();
                    state.lastIndex = next;
                    return next;
                }

                case ClipPickMode.Random:
                default:
                {
                    int next = Random.Range(0, cue.clips.Length);

                    // Try once to avoid immediate repeat when possible.
                    if (cue.clips.Length > 1 && next == state.lastIndex) {
                        next = (next + 1) % cue.clips.Length;
                    }

                    state.lastIndex = next;
                    return next;
                }
            }
        }

        private static void EnsureShuffleBag(AudioCue cue, CueRuntimeState state) {
            state.shuffleBag ??= new Queue<int>();

            if (state.shuffleBag.Count > 0)
                return;

            var temp = new List<int>(cue.clips.Length);
            for (int i = 0; i < cue.clips.Length; i++) {
                temp.Add(i);
            }

            for (int i = temp.Count - 1; i > 0; i--) {
                int j = Random.Range(0, i + 1);
                (temp[i], temp[j]) = (temp[j], temp[i]);
            }

            // Prevent the first item of a new bag from repeating the previous pick.
            if (temp.Count > 1 && temp[0] == state.lastIndex) {
                int last = temp.Count - 1;
                (temp[0], temp[last]) = (temp[last], temp[0]);
            }

            foreach (int bag in temp) {
                state.shuffleBag.Enqueue(bag);
            }
        }

        private PooledAudioSource AcquireSource(AudioCue newCue) {
            // 1. Prefer idle sources.
            foreach (var src in sources.Where(src => !src.IsBusy)) {
                return src;
            }

            // 2. Grow the pool if allowed.
            if (sources.Count < maxPoolSize) {
                return CreateSource();
            }

            // 3. Steal a lower-priority one-shot source.
            PooledAudioSource best = null;

            foreach (var candidate in sources) {
                if (!candidate.IsBusy)
                    return candidate;

                // Do not steal looping sources first.
                if (candidate.IsLooping)
                    continue;

                if (candidate.CurrentPriority > newCue.priority) {
                    if (best == null) {
                        best = candidate;
                        continue;
                    }

                    if (candidate.CurrentPriority > best.CurrentPriority) {
                        best = candidate;
                        continue;
                    }

                    if (candidate.CurrentPriority == best.CurrentPriority &&
                        candidate.StartedAtRealtime < best.StartedAtRealtime) {
                        best = candidate;
                    }
                }
            }

            if (best != null) {
                best.Stop();
                return best;
            }

            // 4. If everything is important, drop the new request.
            return null;
        }

        private PooledAudioSource CreateSource() {
            PooledAudioSource src = Instantiate(sourcePrefab, poolRoot);
            src.gameObject.name = $"PooledAudioSource_{sources.Count}";
            sources.Add(src);
            return src;
        }

        private AudioMixerGroup ResolveMixerGroup(AudioBus bus) {
            return bus switch {
                AudioBus.Ui => uiMixerGroup != null ? uiMixerGroup : sfxMixerGroup,
                AudioBus.Ambience => ambienceMixerGroup != null ? ambienceMixerGroup : sfxMixerGroup,
                _ => sfxMixerGroup
            };
        }

        private void OnSourceReleased(PooledAudioSource source) {
            if (source == null)
                return;

            if (sourceOwners.TryGetValue(source, out AudioCue cue)) {
                if (cue != null && cueStates.TryGetValue(cue, out CueRuntimeState state)) {
                    state.activeVoices = Mathf.Max(0, state.activeVoices - 1);
                }

                sourceOwners.Remove(source);
            }
        }

        private sealed class CueRuntimeState
        {
            public float lastPlayTime = float.NegativeInfinity;
            public int lastIndex = -1;
            public int activeVoices = 0;
            public Queue<int> shuffleBag;
        }
    }
}
