// author: Omnistudio
// version: 2026.01.29

using System;
using System.Collections;
using UnityEngine;

namespace Omnis.Audio
{
    public static class Recorder
    {
        public static AudioClip Start(int lengthSec, int sampleRate = 16000) {
            if (Microphone.devices.Length == 0) {
                Debug.LogError("No microphone devices found.");
                return null;
            }

            var recordingClip = Microphone.Start(null, true, lengthSec, sampleRate);
            if (recordingClip == null) {
                Debug.LogError("Failed to start microphone.");
                return null;
            }

            Debug.Log($"Recording started on default device at {sampleRate} Hz.");
            return recordingClip;
        }

        public static AudioClip Record(MonoBehaviour mono, int lengthSec, int sampleRate = 16000) {
            AudioClip clip = Start(lengthSec, sampleRate);

            if (clip == null) {
                return null;
            }

            mono.StartCoroutine(Stop(lengthSec));
            return clip;
        }

        public static void Stop() {
            Microphone.End(Microphone.devices[0]);
            Debug.Log("Recording stopped.");
        }

        private static IEnumerator Stop(int seconds) {
            yield return new WaitForSeconds(seconds);
            Stop();
        }
    }
}
