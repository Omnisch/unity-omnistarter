// author: Omnistudio
// version: 2026.03.18

using System.Collections;
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
        
        public void LoadSceneByName(string sceneName) {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName) {
            AsyncOperation asyncLoad = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            
            if (asyncLoad == null)
                yield break;
            
            asyncLoad.allowSceneActivation = false;

            if (!string.IsNullOrEmpty(oldSceneName)) {
                yield return transition.TransitionOut(oldSceneName);
            }

            while (!asyncLoad.isDone) {
                if (asyncLoad.progress >= 0.9f) {
                    if (!string.IsNullOrEmpty(oldSceneName)) {
                        UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(oldSceneName);
                    }

                    asyncLoad.allowSceneActivation = true;
                    yield return new WaitForSeconds(redundantLoadTime);
                } else {
                    yield return null;
                }
            }

            yield return transition.TransitionIn(sceneName);
            
            oldSceneName = sceneName;
        }

        private void Start() {
            if (!string.IsNullOrEmpty(startSceneName)) {
                LoadSceneByName(startSceneName);
            }
        }
    }
}
