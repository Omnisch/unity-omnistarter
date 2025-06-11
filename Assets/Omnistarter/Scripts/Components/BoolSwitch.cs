// author: Omnistudio
// version: 2025.06.11

using UnityEngine;

namespace Omnis
{
    public class BoolSwitch : MonoBehaviour
    {
        public bool SwitchGameObjectActive()
        {
            gameObject.SetActive(!gameObject.activeSelf);
            return gameObject.activeSelf;
        }
    }
}
