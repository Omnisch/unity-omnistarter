// author: Omnistudio
// version: 2026.03.15

using UnityEngine;

namespace Omnis.Audio
{
    public partial class AudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSettings settings;
        
        
        // TODO:
        
        
        #region Unity Messages
        private void Awake() {
            if (!EnsureSingleton())
                return;
        }
        #endregion
    }
}
