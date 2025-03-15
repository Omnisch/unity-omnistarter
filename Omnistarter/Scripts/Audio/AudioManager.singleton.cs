// author: Omnistudio
// version: 2025.03.15

namespace Omnis
{
    public partial class AudioManager
    {
        public static AudioManager Instance { get; private set; }

        private bool EnsureSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return false;
            }
            else
            {
                Instance = this;
                StartCoroutine(Util.YieldTweaker.DoAfter(
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
