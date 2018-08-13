using System.Collections;
using System.Collections.Generic;
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

        private UIMover _mover;
        private List<UIFurnitureItem> _furnitureItems = new List<UIFurnitureItem>();

        private void Start()
        {
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
                            _furnitureItems.Add(furnitureItem);
                        }
                    }
                }
            }
        }

        [ContextMenu("Hide")]
        public void Hide()
        {
            Time.timeScale = 1f;
            if (Background != null)
                Background.FadeOut();

            if (_mover != null)
                _mover.MoveToTarget();
        }

        [ContextMenu("Show")]
        public void Show()
        {
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
            FlavorText.text = Room.Instance.FinishText;
            StartCoroutine(StarAnimation());
        }

        private IEnumerator StarAnimation(float startDelay = 0.5f, float starDelay=0.5f)
        {
            yield return new WaitForSecondsRealtime(startDelay);

            var star1 = Room.Instance.Star1();
            var star2 = Room.Instance.Star2();
            var star3 = Room.Instance.Star3();

            if (Star1 != null)
            {
                Star1.gameObject.SetActive(star1);
                yield return new WaitForSecondsRealtime(starDelay);
            }

            if (NextButton != null)
                NextButton.interactable = star1;

            if (Star2 != null)
            {
                Star2.gameObject.SetActive(star2);
                yield return new WaitForSecondsRealtime(starDelay);
            }

            if (Star3 != null)
            {
                Star3.gameObject.SetActive(star3);
            }
        }
    }
}
