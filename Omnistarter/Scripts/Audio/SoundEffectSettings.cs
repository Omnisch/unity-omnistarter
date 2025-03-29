using System.Collections.Generic;
using UnityEngine;

namespace Omnis
{
    [CreateAssetMenu(menuName = "Omnis/Sound Effect Settings", order = 242)]
    public class SoundEffectSettings : ScriptableObject
    {
        public List<SoundEffectId> soundEffects;
    }

    [System.Serializable]
    public struct SoundEffectId
    {
        public SoundEffectName name;
        public AudioClip se;
    }

    public enum SoundEffectName
    {
        
    }
}
