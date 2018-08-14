using UnityEngine;

namespace Assets.Scripts
{
    public class MusicManager : MonoBehaviour
    {
        public AudioClipWithVolume Music;

        void Start()
        {
            if (Music != null)
            {
                if (SoundManager.Instance.MusicHandler == null ||
                    SoundManager.Instance.MusicHandler.Source.clip != Music.Clip)
                {
                    SoundManager.Instance.PlayMusic(Music);
                }
            }
        }
    }
}
