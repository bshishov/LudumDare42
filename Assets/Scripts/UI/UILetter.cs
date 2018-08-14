using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UILetter : MonoBehaviour
    {
        public float Amplitude = 10f;
        public float Speed = 10f;
        public float OffsetMod = 1f;


        private RectTransform _rectTransform;
        private float _offset;
        private Vector2 _origin;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
            _offset = _rectTransform.anchoredPosition.x;
            _origin = _rectTransform.anchoredPosition;
        }

        private void Update()
        {
            var yOffset = Mathf.Sin(_offset * OffsetMod + Time.time * Speed) * Amplitude;
            _rectTransform.anchoredPosition = new Vector2(_origin.x, _origin.y + yOffset);
        }
    }
}
