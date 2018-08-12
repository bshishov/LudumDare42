using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(VoxelObject))]
    public class VoxelFurniture : MonoBehaviour
    {
        public VoxelVolume Volume { get { return _voxelObject.Volume; } }
        public VoxelObject VoxelObject { get { return _voxelObject; } }

        [Header("Animation")]
        public GameObject Representation;
        public AnimationCurve TransformCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public float MoveRotateTime = 0.3f;
        public float MoveTime = 0.1f;

        private VoxelObject _voxelObject;
        private Vector3 _startLocalPosition;
        private Quaternion _startLocalRotation;
        private Vector3 _targetLocalPosition;
        private Quaternion _targetLocalRotation;
        private float _animTime = 0f;
        private bool _isAnimating = false;
        private Vector3 _target;
        private Vector3 _velocity;

        private void Start()
        {
            _voxelObject = GetComponent<VoxelObject>();

            if (Representation != null)
            {
                _startLocalPosition = Representation.transform.localPosition;
                _startLocalRotation = Representation.transform.localRotation;

                _targetLocalPosition = Representation.transform.localPosition;
                _targetLocalRotation = Representation.transform.localRotation;
            }

            _target = transform.position;
        }

        private void Update()
        {
            if (Representation != null && _isAnimating)
            {
                _animTime += Time.deltaTime / MoveRotateTime;
                var lerp = TransformCurve.Evaluate(Mathf.Clamp01(_animTime));

                Representation.transform.localPosition = Vector3.SlerpUnclamped(_startLocalPosition, _targetLocalPosition, lerp);
                Representation.transform.localRotation = Quaternion.SlerpUnclamped(_startLocalRotation, _targetLocalRotation, lerp);

                if (_animTime > 1)
                {
                    _isAnimating = false;
                    _animTime = 0f;
                }
            }

            transform.position = Vector3.SmoothDamp(transform.position, _target, ref _velocity, MoveTime);
        }

        public void RotateYCw()
        {
            _voxelObject.RotateYCw();
            if (Representation != null)
            {
                var pivotCenter = _voxelObject.GetPivotCenter();

                // I was too lazy with this math
                var tmpLoc = Representation.transform.localPosition;
                var tmpRot = Representation.transform.localRotation;

                Representation.transform.localPosition = _targetLocalPosition;
                Representation.transform.localRotation = _targetLocalRotation;

                Representation.transform.RotateAround(pivotCenter, Vector3.up, 90);

                _targetLocalPosition = Representation.transform.localPosition;
                _targetLocalRotation = Representation.transform.localRotation;

                Representation.transform.localPosition = tmpLoc;
                Representation.transform.localRotation = tmpRot;

                _startLocalPosition = tmpLoc;
                _startLocalRotation = tmpRot;
               
                _isAnimating = true;
                _animTime = 0f;
            }
        }

        public void RotateXCw()
        {
            _voxelObject.RotateXCw();
            if (Representation != null)
            {
                var pivotCenter = _voxelObject.GetPivotCenter();

                // I was too lazy with this math
                var tmpLoc = Representation.transform.localPosition;
                var tmpRot = Representation.transform.localRotation;

                Representation.transform.localPosition = _targetLocalPosition;
                Representation.transform.localRotation = _targetLocalRotation;

                Representation.transform.RotateAround(pivotCenter, Vector3.right, 90);

                _targetLocalPosition = Representation.transform.localPosition;
                _targetLocalRotation = Representation.transform.localRotation;

                Representation.transform.localPosition = tmpLoc;
                Representation.transform.localRotation = tmpRot;

                _startLocalPosition = tmpLoc;
                _startLocalRotation = tmpRot;

                _isAnimating = true;
                _animTime = 0f;
            }
        }

        public void MoveSmooth(Vector3 target)
        {
            _target = target;
        }


        void OnDrawGizmos()
        {
            /*
            if (!Application.isPlaying)
                return;

            if (Room.Instance == null)
                return;
            
            var collisions = VoxelSpace.InverseCollistionsA(transform.position, _voxelObject.Volume,
                Room.Instance.transform.position, Room.Instance.FillVolume);

            var collisionsWithObjects = VoxelSpace.CollistionsA(transform.position, _voxelObject.Volume,
                Room.Instance.transform.position, Room.Instance.ObjectsVolume);

            collisions.AddRange(collisionsWithObjects);

            Gizmos.color = new Color(1f, 0, 0, 0.5f);
            foreach (var collision in collisions)
            {
                var wv = VoxelSpace.GetWorldVoxelFromLocalVoxel(transform.position, collision.LocalPosA);
                var center = VoxelSpace.GetWorldPositionCenter(wv);
                Gizmos.DrawCube(center, VoxelSpace.VoxelSize3D);
            }
            */
        }
    }
}
