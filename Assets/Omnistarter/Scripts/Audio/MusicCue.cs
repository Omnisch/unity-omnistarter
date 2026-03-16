// author: ChatGPT
// version: 2026.03.16

using System;
using UnityEngine;

namespace Omnis.Audio
{
    [CreateAssetMenu(menuName = "Omnis/Audio/Music Cue", order = 242)]
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
    
    
    [Serializable]
    public sealed class MusicVariant
    {
        public string id;
        public AudioClip clip;
        
        public float defaultVolume = 1f;
    }
}
