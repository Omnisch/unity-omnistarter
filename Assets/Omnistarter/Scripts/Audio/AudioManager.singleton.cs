using Omnis.Utils;
using UnityEditor;

namespace Omnis.Audio
{
    public partial class AudioManager
    {
        public bool overrideInstance;
    
        public static AudioManager Instance { get; private set; }

        private bool EnsureSingleton() {
            if (Instance == this) return true;
        
            if (Instance != null) {
                if (overrideInstance) {
                    Destroy(Instance.gameObject);
                }
                else {
                    Destroy(gameObject);
                    return false;
                }
            }
        
            Instance = this;
            this.DoWhen(
                () => gameObject.scene.isLoaded,
                () => DontDestroyOnLoad(gameObject));

            return true;
        }

        // private void Awake() {
        //     if (!EnsureSingleton())
        //         return;
        // }
    }


    [CustomEditor(typeof(AudioManager))]
    public class AudioHandlerSingletonEditor : Editor
    {
        public override void OnInspectorGUI() {
            if (target is AudioManager t) {
                if (t.overrideInstance) {
                    EditorGUILayout.HelpBox(
                        "This is a singleton that can be accessed by Instance.\n" +
                        "It WILL OVERRIDE any instance before itself.", MessageType.Info);
                }
                else {
                    EditorGUILayout.HelpBox(
                        "This is a singleton that can be accessed by Instance.\n" +
                        "It MAY BE REPLACED by any instance before itself.", MessageType.Info);
                }
            }

            EditorGUILayout.Space();

            DrawDefaultInspector();
        }
    }
}
