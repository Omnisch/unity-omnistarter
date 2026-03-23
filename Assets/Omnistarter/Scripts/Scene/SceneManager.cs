// author: Omnistudio
// version: 2026.03.18

using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Omnis.SceneManagement
{
    public class SceneManager : MonoBehaviour
    {
        [SerializeField] private string startSceneName;
        [SerializeField] private SceneTransitionBase transition;
        [SerializeField] private float redundantLoadTime = 0.1f;

        public static SceneManager Instance { get; private set; }

        private string oldSceneName;
        private bool isLoading;
        
        public void LoadSceneByName(string sceneName) {
            if (isLoading)
                return;
            StartCoroutine(LoadSceneAsync(sceneName, -1));
        }

        public void LoadSceneByIndex(int sceneIndex) {
            if (isLoading)
                return;
            StartCoroutine(LoadSceneAsync(null, sceneIndex));
        }

        public void LoadNextScene() {
            if (isLoading)
                return;

            int currSceneIndex = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
            int totalSceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
            StartCoroutine(LoadSceneAsync(null,  (currSceneIndex + 1) % totalSceneCount));
        }

        private IEnumerator LoadSceneAsync(string sceneName, int sceneIndex) {
            isLoading = true;

            var loadOp = string.IsNullOrEmpty(sceneName)
                ? UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive)
                : UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (loadOp == null) {
                isLoading = false;
                yield break;
            }

            loadOp.allowSceneActivation = false;

            if (!string.IsNullOrEmpty(oldSceneName)) {
                yield return transition.TransitionOut(oldSceneName);
            }

            while (loadOp.progress < 0.9f) {
                yield return null;
            }

            if (!string.IsNullOrEmpty(oldSceneName)) {
                ShutdownSceneComponents();
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(oldSceneName);
            }

            loadOp.allowSceneActivation = true;

            yield return loadOp;
            yield return new WaitForSecondsRealtime(redundantLoadTime);

            var newScene = string.IsNullOrEmpty(sceneName)
                ? UnityEngine.SceneManagement.SceneManager.GetSceneByBuildIndex(sceneIndex)
                : UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (newScene.IsValid() && newScene.isLoaded) {
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(newScene);
            }

            yield return transition.TransitionIn(sceneName);
            
            oldSceneName = newScene.name;
            isLoading = false;
        }

        private static void ShutdownSceneComponents() {
            var audioListener = FindFirstObjectByType<AudioListener>();
            audioListener.enabled = false;
            
            var mainCamera = FindObjectsByType<Camera>(FindObjectsSortMode.None).First(cam => cam.CompareTag("MainCamera"));
            mainCamera.tag = "Untagged";
        }


        private void Awake() {
            Instance = this;
        }

        private void Start() {
            if (string.IsNullOrEmpty(startSceneName))
                return;
            
            // already additive, so needs no init.
            if (UnityEngine.SceneManagement.SceneManager.loadedSceneCount > 1)
                return;

            LoadSceneByName(startSceneName);
        }
    }
}
