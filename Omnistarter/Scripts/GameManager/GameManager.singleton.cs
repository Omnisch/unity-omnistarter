// author: Omnistudio
// version: 2025.02.19

namespace Omnis
{
    public partial class GameManager
    {
        private static GameManager instance;
        public static GameManager Instance => instance;

        private bool EnsureSingleton()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return false;
            }
            else
            {
                instance = this;
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
