// author: Omnistudio
// version: 2026.03.09

using Omnis.Utils;

namespace Omnis.Audio
{
    public partial class AudioHandler
    {
        public static AudioHandler Agent { get; private set; }

        private bool EnsureSingleton()
        {
            if (Agent != null && Agent != this)
            {
                Destroy(gameObject);
                return false;
            }
            else
            {
                Agent = this;
                this.DoWhen(
                    () => gameObject.scene.isLoaded,
                    () => DontDestroyOnLoad(gameObject));
                return true;
            }
        }

        private void Awake()
        {
            if (!EnsureSingleton())
                return;
        }
    }
}
