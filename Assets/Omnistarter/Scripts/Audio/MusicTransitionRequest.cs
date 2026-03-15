// author: ChatGPT
// version: 2026.03.15

namespace Omnis.Audio
{
    public enum MusicSyncPoint
    {
        Immediate,
        NextBeat,
        NextBar,
        NextPhrase,
        LoopBoundary
    }
    
    public readonly struct MusicTransitionRequest
    {
        public readonly MusicCue cue;
        public readonly int variantIndex;
        public readonly MusicSyncPoint syncPoint;
        public readonly float crossfadeSeconds;

        public MusicTransitionRequest(
            MusicCue cue,
            int variantIndex = -1,
            MusicSyncPoint syncPoint = MusicSyncPoint.NextBar,
            float crossfadeSeconds = 1f)
        {
            this.cue = cue;
            this.variantIndex = variantIndex;
            this.syncPoint = syncPoint;
            this.crossfadeSeconds = crossfadeSeconds;
        }
    }
}
