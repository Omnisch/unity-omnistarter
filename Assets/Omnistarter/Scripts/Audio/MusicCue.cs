// author: ChatGPT
// version: 2026.03.15

using System;
using UnityEngine;

namespace Omnis.Audio
{
    [Serializable]
    public sealed class MusicVariant
    {
        public string id;
        public AudioClip clip;
        
        public float defaultVolume = 1f;
    }
    
    [CreateAssetMenu(menuName = "Omnis/Audio/Music Cue")]
    public class MusicCue : ScriptableObject
    {
        public string id;
        public MusicVariant[] variants;

        public double bpm = 120.0;
        public int beatsPerBar = 4;
        public int barsPerPhrase = 4;

        public bool loop = true;
        
        public int defaultVariantIndex = 0;
        public float defaultCrossfadeSeconds = 1f;
    }
}
