// author: ChatGPT
// version: 2026.03.15

using UnityEngine;

namespace Omnis.Audio
{
    public readonly struct AudioPlayRequest
    {
        public readonly Vector3 Position;
        public readonly Transform FollowTarget;
        public readonly float VolumeMultiplier;
        public readonly float PitchMultiplier;
        public readonly bool Override2D;
        public readonly bool Is2DOverride;

        public AudioPlayRequest(
            Vector3 position,
            Transform followTarget = null,
            float volumeMultiplier = 1f,
            float pitchMultiplier = 1f,
            bool override2D = false,
            bool is2DOverride = false)
        {
            Position = position;
            FollowTarget = followTarget;
            VolumeMultiplier = volumeMultiplier;
            PitchMultiplier = pitchMultiplier;
            Override2D = override2D;
            Is2DOverride = is2DOverride;
        }
    }
}
