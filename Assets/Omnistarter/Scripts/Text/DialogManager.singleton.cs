namespace Omnis.Text
{
    public partial class DialogManager
    {
        public static DialogManager Instance { get; private set; }

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
                StartCoroutine(Omnis.Utils.YieldHelper.DoWhen(
                    () => gameObject.scene.isLoaded,
                    () => DontDestroyOnLoad(gameObject)));
                return true;
            }
        }

        // written in DialogManager.cs
        //private void Awake()
        //{
        //    if (!EnsureSingleton())
        //        return;
        //}
    }
}
