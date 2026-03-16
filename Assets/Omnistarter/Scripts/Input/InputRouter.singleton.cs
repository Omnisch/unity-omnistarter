using Omnis.Utils;

namespace Omnis
{
    public partial class InputRouter
    {
        public static InputRouter Instance { get; private set; }

        private bool EnsureSingleton() {
            if (Instance != null && Instance != this) {
                Destroy(gameObject);
                return false;
            }
            else {
                Instance = this;
                this.DoWhen(
                    () => gameObject.scene.isLoaded,
                    () => DontDestroyOnLoad(gameObject));
                return true;
            }
        }

        //private void Awake()
        //{
        //    if (!EnsureSingleton())
        //        return;
        //}
    }
}
