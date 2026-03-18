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
        
        private string oldSceneName;
        private bool isLoading;
        
        public void LoadSceneByName(string sceneName) {
            if (isLoading)
                return;
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName) {
            isLoading = true;

            var loadOp = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
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

            var newScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);
            if (newScene.IsValid() && newScene.isLoaded) {
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(newScene);
            }

            yield return new WaitForSecondsRealtime(redundantLoadTime);

            yield return transition.TransitionIn(sceneName);
            
            oldSceneName = sceneName;
            isLoading = false;
        }

        private static void ShutdownSceneComponents() {
            var audioListener = FindFirstObjectByType<AudioListener>();
            audioListener.enabled = false;
            
            var mainCamera = FindObjectsByType<Camera>(FindObjectsSortMode.None).First(cam => cam.CompareTag("MainCamera"));
            mainCamera.tag = "Untagged";
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
