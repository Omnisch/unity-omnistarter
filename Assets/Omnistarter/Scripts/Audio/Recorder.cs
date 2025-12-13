// author: Omnistudio
// version: 2025.12.14

using UnityEngine;

namespace Omnis.Audio
{
    public static class Recorder
    {
        public static readonly int ClipLengthSeconds = 10;

        public static AudioClip Start(int sampleRate = 16000) {
            if (Microphone.devices.Length == 0) {
                Debug.LogError("No microphone devices found.");
                return null;
            }

            var recordingClip = Microphone.Start(null, true, ClipLengthSeconds, sampleRate);
            if (recordingClip == null) {
                Debug.LogError("Failed to start microphone.");
                return null;
            }

            Debug.Log($"Recording started on default device at {sampleRate} Hz.");
            return recordingClip;
        }

        public static void Stop() {
            Microphone.End(Microphone.devices[0]);
            Debug.Log("Recording stopped.");
        }
    }
}
