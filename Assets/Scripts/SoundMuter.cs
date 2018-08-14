using UnityEngine;

namespace Assets.Scripts
{
    public class SoundMuter : MonoBehaviour
    {
        public void Toggle()
        {
            AudioListener.volume = 1 - AudioListener.volume;
        }
    }
}
