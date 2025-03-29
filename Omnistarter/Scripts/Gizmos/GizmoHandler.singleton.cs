namespace Omnis.Gizmos
{
    public partial class GizmoHandler
    {
        public static GizmoHandler Agent { get; private set; }

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
                StartCoroutine(Omnis.Utils.YieldHelper.DoWhen(
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
