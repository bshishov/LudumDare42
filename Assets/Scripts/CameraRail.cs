using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraRail : Singleton<CameraRail>
    {
        public List<Vector3> Positions;
        public Vector3 LookAt = Vector3.zero;
        public float TransitionTime = 3f;
        public AnimationCurve TransitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Transform _cameraTransform;
        private int _currentIndex;
        private int _targetIndex;
        private float _anim = 0;

        private void Start()
        {
            _currentIndex = 0;
            _targetIndex = 0;
            _cameraTransform = Camera.main.transform;
            SetCameraPos(Positions[_currentIndex]);
        }

        private void Update()
        {
            if (_targetIndex != _currentIndex)
            {
                _anim += Time.deltaTime / TransitionTime;

                var t = TransitionCurve.Evaluate(Mathf.Clamp01(_anim));

                var sourcePos = Positions[_currentIndex];
                var targetPos = Positions[_targetIndex];

                var xz1 = new Vector3(sourcePos.x, 0, sourcePos.z);
                var xz2 = new Vector3(targetPos.x, 0, targetPos.z);

                var lerpPos = Vector3.SlerpUnclamped(xz1, xz2, t);
                lerpPos.y = Mathf.Lerp(sourcePos.y, targetPos.y, t);
                SetCameraPos(lerpPos);

                if (_anim >= 1f)
                {
                    _anim = 0;
                    _currentIndex = _targetIndex;
                }
            }
            else
            {
                if (Input.GetButtonDown("CameraLeft"))
                    Prev();

                if (Input.GetButtonDown("CameraRight"))
                    Next();
            }
        }

        [ContextMenu("Next")]
        public void Next()
        {
            _targetIndex = NextIndex();
            _anim = 0;
        }

        [ContextMenu("Prev")]
        public void Prev()
        {
            _targetIndex = PrevIndex();
            _anim = 0;
        }

        private int NextIndex()
        {
            return (_currentIndex + 1) % Positions.Count;
        }

        private int PrevIndex()
        {
            return (_currentIndex + Positions.Count - 1) % Positions.Count;
        }

        private void SetCameraPos(Vector3 p)
        {
            _cameraTransform.position = p;
            _cameraTransform.LookAt(Vector3.zero);
        }

        void OnDrawGizmos()
        {
            for (var i = 0; i < Positions.Count; i++)
            {
                var p1 = Positions[i];
                var p2 = Positions[(i + 1) % Positions.Count];
                Gizmos.color = Color.magenta;
                Gizmos.DrawSphere(p1, 0.1f);

                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(p1, p2);
            }
        }
    }
}
