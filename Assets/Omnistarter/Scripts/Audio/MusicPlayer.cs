// author: Omnistudio
// version: 2026.03.15

using System;
using System.Collections;
using System.Collections.Generic;
using Omnis.Utils;
using UnityEngine;
using UnityEngine.Audio;

namespace Omnis.Audio
{
    public sealed class MusicPlayer : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private AudioMixerGroup musicMixerGroup;
        [SerializeField] private Transform laneARoot;
        [SerializeField] private Transform laneBRoot;

        [Header("Scheduling")]
        [SerializeField, Min(0.02f)] private double scheduleLeadTime = 0.10;
        [SerializeField, Min(0f)] private float defaultFadeSeconds = 1f;

        public MusicCue CurrentCue => active.cue;
        public int CurrentVariantIndex => active.currentVariantIndex;

        private Lane laneA;
        private Lane laneB;
        private Lane active;
        private Lane inactive;

        private void Awake() {
            if (laneARoot == null) {
                laneARoot = new GameObject("LaneA").transform;
                laneARoot.SetParent(transform, false);
            }

            if (laneBRoot == null) {
                laneBRoot = new GameObject("LaneB").transform;
                laneBRoot.SetParent(transform, false);
            }

            laneA = new Lane(laneARoot);
            laneB = new Lane(laneBRoot);

            active = laneA;
            inactive = laneB;
        }

        public void Play(MusicCue cue) {
            if (!IsCueValid(cue))
                return;

            StopLaneNow(laneA, true);
            StopLaneNow(laneB, true);

            active = laneA;
            inactive = laneB;

            int variantIndex = ResolveVariantIndex(cue, -1);
            double startDspTime = AudioSettings.dspTime + scheduleLeadTime;

            PrepareLane(active, cue, startDspTime, variantIndex, 1f);
        }

        public void Transition(in MusicTransitionRequest request) {
            if (!IsCueValid(request.cue))
                return;

            float fadeSeconds = request.crossfadeSeconds > 0f
                ? request.crossfadeSeconds
                : request.cue.defaultCrossfadeSeconds > 0f
                    ? request.cue.defaultCrossfadeSeconds
                    : defaultFadeSeconds;

            int targetVariantIndex = ResolveVariantIndex(request.cue, request.variantIndex);

            // No current music: behave like Play().
            if (!active.HasCue) {
                double startDspTime = AudioSettings.dspTime + scheduleLeadTime;
                PrepareLane(active, request.cue, startDspTime, targetVariantIndex, 1f);
                return;
            }

            // Same cue: only switch aligned variants inside the active lane.
            if (active.cue == request.cue) {
                SetVariant(targetVariantIndex, fadeSeconds);
                return;
            }

            double targetDspTime = GetTargetDspTime(request.syncPoint);

            StopLaneNow(inactive, true);
            PrepareLane(inactive, request.cue, targetDspTime, targetVariantIndex, 0f);

            Lane fadingOut = active;
            Lane fadingIn = inactive;

            active = fadingIn;
            inactive = fadingOut;

            StartLaneCrossfade(fadingOut, fadingIn, targetDspTime, fadeSeconds);
        }

        public void SetVariant(int variantIndex, float fadeSeconds = 1f) {
            if (!active.HasCue)
                return;

            int targetIndex = ResolveVariantIndex(active.cue, variantIndex);
            if (targetIndex == active.currentVariantIndex)
                return;

            StartVariantCrossfade(active, targetIndex, fadeSeconds);
        }

        public void StopMusic(float fadeSeconds = 1f) {
            if (!active.HasCue)
                return;

            StopLaneFadeRoutine(active);
            StopVariantFadeRoutine(active);
            StartCoroutine(FadeOutAndStopLaneCoroutine(active, fadeSeconds));
        }

        private static bool IsCueValid(MusicCue cue)
            => cue != null && cue.variants is { Length: > 0 };

        private static int ResolveVariantIndex(MusicCue cue, int requestedIndex) {
            if (cue == null || cue.variants == null || cue.variants.Length == 0)
                return -1;

            return Mathf.Clamp(
                requestedIndex >= 0 ? requestedIndex : cue.defaultVariantIndex,
                0,
                cue.variants.Length - 1);
        }

        private void PrepareLane(Lane lane, MusicCue cue, double startDspTime, int initialVariantIndex, float initialLaneVolume) {
            StopLaneFadeRoutine(lane);
            StopVariantFadeRoutine(lane);
            StopLaneNow(lane, clearCue: false);

            EnsureSourceCount(lane, cue.variants.Length);

            lane.cue = cue;
            lane.startDspTime = startDspTime;
            lane.currentVariantIndex = initialVariantIndex;
            lane.laneVolume = Mathf.Clamp01(initialLaneVolume);
            lane.variantWeights = new float[cue.variants.Length];

            for (int i = 0; i < cue.variants.Length; i++) {
                lane.variantWeights[i] = i == initialVariantIndex ? 1f : 0f;

                AudioSource src = lane.Sources[i];
                src.outputAudioMixerGroup = musicMixerGroup;
                src.clip = cue.variants[i].clip;
                src.loop = cue.loop;
                src.playOnAwake = false;
                src.spatialBlend = 0f;
                src.volume = 0f;

                if (src.clip != null) {
                    src.PlayScheduled(startDspTime);
                }
            }

            // Stop and clear unused sources if this lane had more sources than current variant count.
            for (int i = cue.variants.Length; i < lane.Sources.Count; i++) {
                AudioSource src = lane.Sources[i];
                src.Stop();
                src.clip = null;
                src.volume = 0f;
            }

            RefreshLaneVolumes(lane);
        }

        private static void EnsureSourceCount(Lane lane, int neededCount) {
            while (lane.Sources.Count < neededCount) {
                int index = lane.Sources.Count;
                GameObject go = new GameObject($"Variant_{index}");
                go.transform.SetParent(lane.Root, false);

                AudioSource src = go.AddComponent<AudioSource>();
                src.playOnAwake = false;
                src.loop = true;
                src.spatialBlend = 0f;

                lane.Sources.Add(src);
            }
        }

        private static void RefreshLaneVolumes(Lane lane) {
            if (!lane.HasCue || lane.variantWeights == null)
                return;

            int count = Mathf.Min(lane.cue.variants.Length, lane.Sources.Count);

            for (int i = 0; i < count; i++) {
                AudioSource src = lane.Sources[i];
                float variantBase = lane.cue.variants[i].defaultVolume;
                float finalVolume = lane.laneVolume * lane.variantWeights[i] * variantBase;
                src.volume = finalVolume;
            }
        }

        private void StartVariantCrossfade(Lane lane, int targetVariantIndex, float fadeSeconds) {
            StopVariantFadeRoutine(lane);
            lane.variantFadeRoutine = StartCoroutine(VariantCrossfadeCoroutine(lane, targetVariantIndex, fadeSeconds));
        }

        private static IEnumerator VariantCrossfadeCoroutine(Lane lane, int targetVariantIndex, float fadeSeconds) {
            if (!lane.HasCue)
                yield break;

            int oldIndex = lane.currentVariantIndex;
            lane.currentVariantIndex = targetVariantIndex;

            if (oldIndex == targetVariantIndex)
                yield break;

            if (fadeSeconds <= 0f) {
                for (int i = 0; i < lane.variantWeights.Length; i++) {
                    lane.variantWeights[i] = i == targetVariantIndex ? 1f : 0f;
                }

                RefreshLaneVolumes(lane);
                lane.variantFadeRoutine = null;
                yield break;
            }

            float[] fromWeights = new float[lane.variantWeights.Length];
            Array.Copy(lane.variantWeights, fromWeights, lane.variantWeights.Length);

            yield return YieldHelper.EEase(
                Easing.Linear,
                value => {
                    for (int i = 0; i < lane.variantWeights.Length; i++) {
                        float target = i == targetVariantIndex ? 1f : 0f;
                        lane.variantWeights[i] = Mathf.Lerp(fromWeights[i], target, value);
                    }
                    
                    RefreshLaneVolumes(lane);
                }
            );
            
            lane.variantFadeRoutine = null;
        }

        private void StartLaneCrossfade(Lane fadingOut, Lane fadingIn, double startDspTime, float fadeSeconds) {
            StopLaneFadeRoutine(fadingOut);
            StopLaneFadeRoutine(fadingIn);

            fadingOut.laneFadeRoutine =
                StartCoroutine(LaneCrossfadeCoroutine(fadingOut, fadingIn, startDspTime, fadeSeconds));
        }

        private IEnumerator LaneCrossfadeCoroutine(Lane fadingOut, Lane fadingIn, double startDspTime, float fadeSeconds) {
            while (AudioSettings.dspTime < startDspTime) {
                yield return null;
            }

            if (fadeSeconds <= 0f) {
                fadingOut.laneVolume = 0f;
                RefreshLaneVolumes(fadingOut);
                StopLaneNow(fadingOut, clearCue: true);

                fadingIn.laneVolume = 1f;
                RefreshLaneVolumes(fadingIn);

                fadingOut.laneFadeRoutine = null;
                yield break;
            }

            float fromOut = fadingOut.laneVolume;
            float fromIn = fadingIn.laneVolume;

            yield return YieldHelper.EEase(
                Easing.Linear,
                value => {
                    fadingOut.laneVolume = Mathf.Lerp(fromOut, 0f, value);
                    fadingIn.laneVolume = Mathf.Lerp(fromIn, 1f, value);
                    
                    RefreshLaneVolumes(fadingOut);
                    RefreshLaneVolumes(fadingIn);
                }
            );

            StopLaneNow(fadingOut, clearCue: true);
            fadingOut.laneFadeRoutine = null;
        }

        private IEnumerator FadeOutAndStopLaneCoroutine(Lane lane, float fadeSeconds) {
            if (!lane.HasCue)
                yield break;

            if (fadeSeconds <= 0f) {
                StopLaneNow(lane, true);
                yield break;
            }

            float from = lane.laneVolume;

            yield return YieldHelper.EEase(
                Easing.Linear,
                value => {
                    lane.laneVolume = Mathf.Lerp(from, 0f, value);
                    RefreshLaneVolumes(lane);
                },
                () => StopLaneNow(lane, true),
                fadeSeconds
            );
        }

        private void StopLaneNow(Lane lane, bool clearCue) {
            StopLaneFadeRoutine(lane);
            StopVariantFadeRoutine(lane);

            foreach (var source in lane.Sources) {
                source.Stop();
                source.volume = 0f;
            }

            lane.laneVolume = 0f;

            if (clearCue) {
                lane.ClearCue();
            }
        }

        private void StopLaneFadeRoutine(Lane lane) {
            if (lane.laneFadeRoutine != null) {
                StopCoroutine(lane.laneFadeRoutine);
                lane.laneFadeRoutine = null;
            }
        }

        private void StopVariantFadeRoutine(Lane lane) {
            if (lane.variantFadeRoutine != null) {
                StopCoroutine(lane.variantFadeRoutine);
                lane.variantFadeRoutine = null;
            }
        }

        private double GetTargetDspTime(MusicSyncPoint syncPoint) {
            double now = AudioSettings.dspTime + scheduleLeadTime;

            if (!active.HasCue)
                return now;

            if (syncPoint == MusicSyncPoint.Immediate)
                return now;

            MusicCue cue = active.cue;
            double origin = active.startDspTime;

            double beatDuration = 60.0 / Math.Max(1.0, cue.bpm);
            double barDuration = beatDuration * Mathf.Max(1, cue.beatsPerBar);
            double phraseDuration = barDuration * Mathf.Max(1, cue.barsPerPhrase);

            switch (syncPoint) {
                case MusicSyncPoint.NextBeat:
                    return GetNextBoundary(now, origin, beatDuration);

                case MusicSyncPoint.NextBar:
                    return GetNextBoundary(now, origin, barDuration);

                case MusicSyncPoint.NextPhrase:
                    return GetNextBoundary(now, origin, phraseDuration);

                case MusicSyncPoint.LoopBoundary:
                {
                    double loopDuration = GetCueDurationSeconds(cue);
                    if (loopDuration <= 0.0)
                        return now;

                    return GetNextBoundary(now, origin, loopDuration);
                }

                default:
                    return now;
            }
        }

        private static double GetCueDurationSeconds(MusicCue cue) {
            if (!IsCueValid(cue))
                return 0.0;

            AudioClip reference = cue.variants[0].clip;

            if (reference == null)
                return 0.0;
            
            return reference.length;
        }

        private static double GetNextBoundary(double now, double origin, double interval) {
            if (interval <= 0.0)
                return now;

            if (now <= origin)
                return origin;

            double steps = Math.Ceiling((now - origin) / interval);
            double target = origin + steps * interval;

            while (target < now) {
                target += interval;
            }

            return target;
        }
        
        
        private sealed class Lane
        {
            public Transform Root { get; }
            public List<AudioSource> Sources { get; } = new List<AudioSource>();

            public MusicCue cue;
            public double startDspTime;
            public int currentVariantIndex = -1;
            public float laneVolume = 1f;
            public float[] variantWeights;

            public Coroutine laneFadeRoutine;
            public Coroutine variantFadeRoutine;

            public bool HasCue => cue != null;

            public Lane(Transform root) {
                Root = root;
            }

            public void ClearCue() {
                cue = null;
                startDspTime = 0.0;
                currentVariantIndex = -1;
                variantWeights = null;
            }
        }
    }
}
