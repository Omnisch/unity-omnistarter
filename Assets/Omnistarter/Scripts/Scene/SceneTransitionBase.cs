// author: Omnistudio
// version: 2026.03.18

using System.Collections;
using UnityEngine;

namespace Omnis.SceneManagement
{
    public abstract class SceneTransitionBase : MonoBehaviour
    {
        public abstract IEnumerator TransitionIn(string sceneName);
        public abstract IEnumerator TransitionOut(string sceneName);
    }
}
