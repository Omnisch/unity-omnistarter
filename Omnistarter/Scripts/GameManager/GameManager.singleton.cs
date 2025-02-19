// author: Omnistudio
// version: 2025.02.19

namespace Omnis
{
    public partial class GameManager
    {
        public static GameManager Instance { get; private set; }

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
                StartCoroutine(YieldTweaker.DoAfter(
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
