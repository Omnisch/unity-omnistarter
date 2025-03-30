// author: Omnistudio
// version: 2025.03.30

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
                StartCoroutine(Utils.YieldHelper.DoWhen(
                    () => gameObject.scene.isLoaded,
                    () => DontDestroyOnLoad(gameObject)));
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
