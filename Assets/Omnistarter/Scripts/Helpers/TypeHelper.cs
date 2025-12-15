// author: Omnistudio
// version: 2025.12.14

using System;
using UnityEngine;

namespace Omnis.Utils
{
    public static class TypeHelper
    {
        public static bool IsIntegerLike(object v) =>
            v is sbyte || v is byte || v is short || v is ushort || v is int || v is uint || v is long || v is ulong;

        public static bool ToBool(object v) {
            if (v is bool b) return b;
            if (IsIntegerLike(v)) return Convert.ToInt64(v) != 0;
            if (v is string s) return bool.TryParse(s, out var r) && r;
            return false;
        }



        #region Audio (Written by ChatGPT)
        public static AudioClip MakeAudioClipFromBase64(string dataStr, int frequency, int channel = 1) {
            byte[] dataBytes = Convert.FromBase64String(dataStr);
            float[] dataFloats = new float[dataBytes.Length / 2];
            for (int i = 0; i < dataFloats.Length; i++) {
                short sample = BitConverter.ToInt16(dataBytes, i * 2);
                dataFloats[i] = sample / (float)short.MaxValue;
            }
            AudioClip dataClip = AudioClip.Create("DecodedVoice", dataFloats.Length, channel, frequency, false);
            dataClip.SetData(dataFloats, 0);
            return dataClip;
        }


        // AudioClip -> 16-bit PCM raw bytes (interleaved, little-endian)
        public static byte[] AudioClipToRawPcm16(AudioClip clip) {
            if (clip == null) throw new ArgumentNullException(nameof(clip));

            int samples = clip.samples * clip.channels;
            float[] floatData = new float[samples];
            clip.GetData(floatData, 0);

            byte[] bytes = new byte[samples * 2]; // 16-bit = 2 bytes
            int offset = 0;

            for (int i = 0; i < samples; i++) {
                // Clamp to [-1, 1] and scale
                float f = Mathf.Clamp(floatData[i], -1f, 1f);
                short s = (short)Mathf.RoundToInt(f * short.MaxValue);

                // Little-endian
                bytes[offset++] = (byte)(s & 0xFF);
                bytes[offset++] = (byte)((s >> 8) & 0xFF);
            }

            return bytes;
        }

        public static string AudioClipToRawBase64(AudioClip clip) {
            byte[] pcm = AudioClipToRawPcm16(clip);
            return Convert.ToBase64String(pcm);
        }


        // AudioClip -> WAV (RIFF/WAVE, PCM 16-bit) bytes.
        public static byte[] AudioClipToWav(AudioClip clip) {
            if (clip == null) throw new ArgumentNullException(nameof(clip));

            int channels = clip.channels;
            int sampleRate = clip.frequency;

            int sampleCount = clip.samples * channels;
            float[] floatData = new float[sampleCount];
            clip.GetData(floatData, 0);

            // 16-bit PCM data
            byte[] pcmData = new byte[sampleCount * 2];
            int pcmOffset = 0;

            for (int i = 0; i < sampleCount; i++) {
                float f = Mathf.Clamp(floatData[i], -1f, 1f);
                short s = (short)Mathf.RoundToInt(f * short.MaxValue);

                pcmData[pcmOffset++] = (byte)(s & 0xFF);
                pcmData[pcmOffset++] = (byte)((s >> 8) & 0xFF);
            }

            // WAV header is 44 bytes
            byte[] wav = new byte[44 + pcmData.Length];

            // RIFF header
            // ChunkID "RIFF"
            wav[0] = (byte)'R';
            wav[1] = (byte)'I';
            wav[2] = (byte)'F';
            wav[3] = (byte)'F';

            // ChunkSize = 36 + Subchunk2Size
            int fileSizeMinus8 = 36 + pcmData.Length;
            WriteInt32LE(wav, 4, fileSizeMinus8);

            // Format "WAVE"
            wav[8] = (byte)'W';
            wav[9] = (byte)'A';
            wav[10] = (byte)'V';
            wav[11] = (byte)'E';

            // Subchunk1ID "fmt "
            wav[12] = (byte)'f';
            wav[13] = (byte)'m';
            wav[14] = (byte)'t';
            wav[15] = (byte)' ';

            // Subchunk1Size = 16 for PCM
            WriteInt32LE(wav, 16, 16);

            // AudioFormat = 1 (PCM)
            WriteInt16LE(wav, 20, 1);

            // NumChannels
            WriteInt16LE(wav, 22, (short)channels);

            // SampleRate
            WriteInt32LE(wav, 24, sampleRate);

            // ByteRate = SampleRate * NumChannels * BitsPerSample/8
            int byteRate = sampleRate * channels * 2;
            WriteInt32LE(wav, 28, byteRate);

            // BlockAlign = NumChannels * BitsPerSample/8
            short blockAlign = (short)(channels * 2);
            WriteInt16LE(wav, 32, blockAlign);

            // BitsPerSample = 16
            WriteInt16LE(wav, 34, 16);

            // Subchunk2ID "data"
            wav[36] = (byte)'d';
            wav[37] = (byte)'a';
            wav[38] = (byte)'t';
            wav[39] = (byte)'a';

            // Subchunk2Size = NumSamples * NumChannels * BitsPerSample/8
            WriteInt32LE(wav, 40, pcmData.Length);

            // PCM data
            Buffer.BlockCopy(pcmData, 0, wav, 44, pcmData.Length);

            return wav;
        }

        public static string AudioClipToWavBase64(AudioClip clip) {
            byte[] wavBytes = AudioClipToWav(clip);
            return Convert.ToBase64String(wavBytes);
        }

        private static void WriteInt16LE(byte[] buffer, int offset, short value) {
            buffer[offset] = (byte)(value & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
        }

        private static void WriteInt32LE(byte[] buffer, int offset, int value) {
            buffer[offset] = (byte)(value & 0xFF);
            buffer[offset + 1] = (byte)((value >> 8) & 0xFF);
            buffer[offset + 2] = (byte)((value >> 16) & 0xFF);
            buffer[offset + 3] = (byte)((value >> 24) & 0xFF);
        }
        #endregion



        public static TField GetFieldByName<TStruct, TField>(this TStruct rootObj, string fieldName)
        {
            object currObj = rootObj;
            Type currType = typeof(TStruct);

            foreach (var layer in fieldName.Split('.'))
            {
                if (currObj == null)
                {
                    Debug.LogError($"Null encountered at {layer} in {fieldName}");
                    return default;
                }

                var field = currType.GetField(layer);
                if (field != null)
                {
                    currObj = field.GetValue(currObj);
                    currType = field.FieldType;
                    continue;
                }

                Debug.LogError($"No field named \"{layer}\" in type {currType.Name}");
                return default;
            }

            try
            {
                return (TField)currObj;
            }
            catch (Exception e)
            {
                Debug.LogError($"Type mismatch: {e}");
                return default;
            }
        }
    }
}
