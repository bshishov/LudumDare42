using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UILevelIcon : MonoBehaviour
    {
        public string LevelName;
        public Image Star1;
        public Image Star2;
        public Image Star3;

        private void Start()
        {
            var star1 = PlayerPrefs.GetInt(string.Format("{0}_star1", LevelName), 0);
            var star2 = PlayerPrefs.GetInt(string.Format("{0}_star2", LevelName), 0);
            var star3 = PlayerPrefs.GetInt(string.Format("{0}_star3", LevelName), 0);

            if (Star1 != null)
                Star1.gameObject.SetActive(star1 > 0);
            if (Star2 != null)
                Star2.gameObject.SetActive(star2 > 0);
            if (Star3 != null)
                Star3.gameObject.SetActive(star3 > 0);
        }

        public void LoadLevel()
        {
            SceneManager.LoadScene(LevelName);
        }
    }
}
