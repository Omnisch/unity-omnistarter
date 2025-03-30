using System.Collections.Generic;
using UnityEngine;

namespace Omnis.Audio
{
    [CreateAssetMenu(menuName = "Omnis/Audio Settings", order = 242)]
    public class AudioSettings : ScriptableObject
    {
        public List<SeInfo> soundEffects;
    }

    [System.Serializable]
    public struct SeInfo
    {
        public string name;
        public AudioClip audio;
    }
}
