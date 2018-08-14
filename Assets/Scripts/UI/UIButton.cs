using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.UI
{
    public class UIButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public bool UpdateCursors = true;

        [Header("Sounds")]
        public AudioClipWithVolume HoverSound;
        public AudioClipWithVolume ClickSound;

        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.Instance.Play(ClickSound);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            SoundManager.Instance.Play(HoverSound);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
        }
    }
}
