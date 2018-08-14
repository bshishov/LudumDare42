using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(UIMover))]
    public class UITablet : Singleton<UITablet>
    {
        public Text CaptionText;
        public Text FlavorText;
        public UICanvasGroupFader Background;
        public UICanvasGroupFader StarsFader;
        public UICanvasGroupFader LestGo;
        public RectTransform FurnitureList;
        public GameObject FurnitureItemPrefab;
        public Button NextButton;
        public Image Star1;
        public Image Star2;
        public Image Star3;

        [Header("Sounds")]
        public AudioClipWithVolume OpenSound;
        public AudioClipWithVolume CloseSound;
        public AudioClipWithVolume CheckMarkSound;
        public AudioClipWithVolume Star1Sound;
        public AudioClipWithVolume Star2Sound;
        public AudioClipWithVolume Star3Sound;
        public AudioClipWithVolume LevelLoaded;
        public AudioClipWithVolume Music;

        private UIMover _mover;
        private readonly List<UIFurnitureItem> _furnitureItems = new List<UIFurnitureItem>();

        private void Start()
        {
            SoundManager.Instance.Play(LevelLoaded);
            Time.timeScale = 0f;
            _mover = GetComponent<UIMover>();

            if (Star1 != null)
                Star1.gameObject.SetActive(false);

            if (Star2 != null)
                Star2.gameObject.SetActive(false);

            if (Star3 != null)
                Star3.gameObject.SetActive(false);

            if (NextButton != null)
                NextButton.interactable = false;

            if (FlavorText != null)
                FlavorText.text = Room.Instance.Text;

            if (FurnitureList != null && FurnitureItemPrefab != null)
            {
                var required = new List<Furniture>();

                foreach (var task in Room.Instance.FurnitureTasks)
                {
                    for (var i = 0; i < task.Amount; i++)
                        required.Add(task.Furniture);
                }

                var pool = Room.Instance.FurniturePool;
                foreach (var roomTask in pool)
                {
                    for(var i=0; i<roomTask.Amount; i++)
                    {
                        var go = Instantiate(FurnitureItemPrefab, FurnitureList);
                        var furnitureItem = go.GetComponent<UIFurnitureItem>();
                        if (furnitureItem != null)
                        {
                            furnitureItem.Setup(roomTask.Furniture);

                            var r = required.FirstOrDefault(f => f.Equals(furnitureItem.Furniture));
                            if (r != null)
                            {
                                furnitureItem.Required = true;
                                required.Remove(r);
                            }

                            _furnitureItems.Add(furnitureItem);
                        }
                    }
                }
            }
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            SoundManager.Instance.Play(CloseSound);
            Time.timeScale = 1f;
            if (Background != null)
                Background.FadeOut();

            if (_mover != null)
                _mover.MoveToTarget();
        }

        [ContextMenu("Show")]
        public void Show()
        {
            SoundManager.Instance.Play(OpenSound);
            Time.timeScale = 0f;
            if (Background != null)
            {
                if(Background.State == UICanvasGroupFader.FaderState.FadedIn)
                    return;
                Background.FadeIn();
            }

            if(_mover != null)
                _mover.MoveToSource();
        }

        public void Next()
        {
            var nextScene = Room.Instance.NextScene;
            if (!string.IsNullOrEmpty(nextScene))
            {
                SceneManager.LoadScene(nextScene);
            }
            else
            {
                Debug.LogWarning("Next scene is not set");
            }
        }

        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        [ContextMenu("Show End")]
        public void ShowEnd()
        {
            Show();

            if(LestGo != null)
                LestGo.FadeOut();

            if(StarsFader != null)
                StarsFader.FadeIn();

            CaptionText.text = "Results";
            var star1 = Room.Instance.Star1();

            if (star1)
                FlavorText.text = Room.Instance.FinishText;
            else
                FlavorText.text = "All yellow items should be placed";

            StartCoroutine(StarAnimation());
        }

        private IEnumerator StarAnimation(float startDelay = 0.5f, float starDelay=0.5f, float furnitureDelay=0.2f)
        {
            yield return new WaitForSecondsRealtime(startDelay);

            var placedAll = Room.Instance.GetPlacedFurniture();
            var pitch = 1f;
            foreach (var uiFurnitureItem in _furnitureItems)
            {
                var placed = placedAll.FirstOrDefault(p => p == uiFurnitureItem.Furniture);
                if (placed != null)
                {
                    placedAll.Remove(placed);
                    uiFurnitureItem.SetCheckmark(true);
                    SoundManager.Instance.Play(CheckMarkSound, pitch: pitch);
                    pitch += 0.01f;
                }
                yield return new WaitForSecondsRealtime(furnitureDelay);
            }

            var star1 = Room.Instance.Star1();
            var star2 = false;
            var star3 = false;
            if (star1)
            {
                star2 = Room.Instance.Star2();
                star3 = Room.Instance.Star3();
            }

            // Save results
            var levelName = SceneManager.GetActiveScene().name;
            PlayerPrefs.SetInt(string.Format("{0}_star1", levelName), star1 ? 1 : 0);
            PlayerPrefs.SetInt(string.Format("{0}_star2", levelName), star2 ? 1 : 0);
            PlayerPrefs.SetInt(string.Format("{0}_star3", levelName), star3 ? 1 : 0);

            if (Star1 != null)
            {
                if(star1)
                    SoundManager.Instance.Play(Star1Sound);
                Star1.gameObject.SetActive(star1);
                yield return new WaitForSecondsRealtime(starDelay);
            }

            if (NextButton != null)
                NextButton.interactable = star1;

            if (Star2 != null)
            {
                if(star2)
                    SoundManager.Instance.Play(Star2Sound);
                Star2.gameObject.SetActive(star2);
                yield return new WaitForSecondsRealtime(starDelay);
            }

            if (Star3 != null)
            {
                if (star3)
                    SoundManager.Instance.Play(Star3Sound);
                Star3.gameObject.SetActive(star3);
            }
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("menu");
        }
    }
}
