// author: ChatGPT
// version: 2026.03.15

using UnityEngine;

namespace Omnis.Audio
{
    public enum AudioBus
    {
        Music,
        Sfx,
        Ui,
        Ambience
    }

    public enum ClipPickMode
    {
        Random,
        ShuffleNoImmediateRepeat,
        Sequential
    }
    
    [CreateAssetMenu(menuName = "Omnis/Audio/Audio Cue")]
    public sealed class AudioCue : ScriptableObject
    {
        public AudioBus bus;
        public AudioClip[] clips;

        public ClipPickMode pickMode = ClipPickMode.ShuffleNoImmediateRepeat;
        
        public Vector2 volumeRange = new Vector2(0.95f, 1f);
        public Vector2 pitchRange = new Vector2(0.97f, 1.03f);

        public bool is2D = true;
        public float spatialBlend = 1f;
        public float minDistance = 1f;
        public float maxDistance = 20f;

        public bool loop = false;
        public int priority = 128;

        public float cooldownSeconds = 0f;
        public int maxSimultaneousVoices = 4;
    }
}
