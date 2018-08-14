using Assets.Scripts.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class UIFurnitureItem : MonoBehaviour
    {
        public Furniture Furniture { get; private set; }
        public Image Icon;
        public Image CheckMark;

        public Color RequiredTint = Color.cyan;

        public bool Required
        {
            get
            {
                return _required;
            }
            set
            {
                if (Icon != null)
                {
                    if (value)
                    {
                        Icon.color = RequiredTint;
                    }
                }
                _required = value;
            }
        }

        private bool _required;

        private void Start()
        {
            SetCheckmark(false);
        }

        public void Setup(Furniture furniture)
        {
            Furniture = furniture;
            if (Icon != null)
                Icon.sprite = furniture.Icon;
        }

        public void SetCheckmark(bool value=true)
        {
            if (CheckMark != null)
            {
                //CheckMark.color = value ? CheckMarkColor : new Color(0, 0, 0, 1);
                CheckMark.gameObject.SetActive(value);
            }
        }
    }
}
