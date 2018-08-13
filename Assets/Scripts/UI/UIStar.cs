using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class UIStar : MonoBehaviour
    {
        public AnimationCurve ScaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float AnimTime;

        private bool _isAnimating;
        private float _anim = 0.5f;
        private RectTransform _rectTransform;

        void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        void Update()
        {
            if (_isAnimating)
            {
                _anim += Time.fixedDeltaTime;
                var t = ScaleCurve.Evaluate(Mathf.Clamp01(_anim));

                _rectTransform.localScale = new Vector3(t, t, t);
                if (_anim > 1)
                    _isAnimating = false;
            }
        }

        void OnEnable()
        {
            _anim = 0;
            _isAnimating = true;
        }
    }
}
