﻿using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class Placer : Singleton<Placer>
    {
        public float RemainingTime { get { return _remainingTime; } }

        [Header("Gameplay")]
        public float Timer = 10f;
        public float RotationMouseTravel = 20f;

        [Header("Visuals")]
        public Material RedVoxel;

        [Header("Sounds")]
        public AudioClipWithVolume RotateSound;
        public AudioClipWithVolume NewSound;
        public AudioClipWithVolume PlaceSound;
        public AudioClipWithVolume SkipSound;
        public AudioClipWithVolume CantPlaceSound;

        private VoxelFurniture _target;
        private Plane _plane = new Plane(Vector3.up, 0f);
        private int _yPos = 0;
        private Mesh _voxelMesh;
        private bool _placeLock;
        private float _remainingTime;
        private bool _isRotating;
        private float _rotationTravelX;
        private float _rotationTravelY;

        private void Start()
        {
            _voxelMesh = VoxelSpace.CreateVoxelMesh();
            GetNextTarget();
        }

        private void GetNextTarget()
        {
            if (Room.Instance.FurnitureStack.Count > 0)
            {
                var furniture = Room.Instance.FurnitureStack.Pop();
                if (furniture.Prefab != null)
                {
                    if(_target != null)
                        _target.SetAppearance(false);

                    var go = GameObject.Instantiate(furniture.Prefab, new Vector3(0, 2, 0), Quaternion.identity);
                    _target = go.GetComponent<VoxelFurniture>();
                    _target.Furniture = furniture;
                    _target.SetAppearance(true);

                    _remainingTime = Timer;

                    SoundManager.Instance.Play(NewSound);
                }
            }
            else
            {
                UITablet.Instance.ShowEnd();
            }
        }

        private void Update()
        {
            if (_target == null || Room.Instance == null)
                return;

            // Timer check
            _remainingTime -= Time.deltaTime;
            if (_remainingTime < 0)
            {
                if (!_placeLock)
                {
                    _placeLock = true;
                    SoundManager.Instance.Play(SkipSound);
                    _target.MoveSmooth(new Vector3(0, 5, 0), () =>
                    {
                        Destroy(_target.gameObject);
                        GetNextTarget();
                        _placeLock = false;
                    });

                }
                return;
            }

            /*
            // Rotation
            if (Input.GetButtonDown("RotateY"))
            {
                SoundManager.Instance.Play(RotateSound);
                _target.RotateYCw();
            }

            if (Input.GetButtonDown("RotateX"))
            {
                SoundManager.Instance.Play(RotateSound);
                _target.RotateXCw();
            }*/

            _isRotating = Input.GetButton("RotateY");
            if (_isRotating)
            {
                //Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                var xDelta = Input.GetAxis("Mouse X");
                var yDelta = Input.GetAxis("Mouse Y");

                if (xDelta > 0 && _rotationTravelX < 0)
                    _rotationTravelX = 0f;

                if (xDelta < 0 && _rotationTravelX > 0)
                    _rotationTravelX = 0f;

                if (yDelta > 0 && _rotationTravelY < 0)
                    _rotationTravelY = 0f;
                
                if (yDelta < 0 && _rotationTravelY > 0)
                    _rotationTravelY = 0f;
                
                _rotationTravelX += xDelta;
                _rotationTravelY += yDelta;

                if (_rotationTravelX > RotationMouseTravel)
                {
                    _rotationTravelX = 0;
                    SoundManager.Instance.Play(RotateSound);
                    _target.RotateYCCw();
                }
                
                if (_rotationTravelX < -RotationMouseTravel)
                {
                    _rotationTravelX = 0;
                    SoundManager.Instance.Play(RotateSound);
                    _target.RotateYCw();
                }

                if (_rotationTravelY > RotationMouseTravel)
                {
                    _rotationTravelY = 0;
                    SoundManager.Instance.Play(RotateSound);
                    _target.RotateXCw();
                }

                if (_rotationTravelY < -RotationMouseTravel)
                {
                    _rotationTravelY = 0;
                    SoundManager.Instance.Play(RotateSound);
                    _target.RotateXCCw();
                }
            }
            else
            {
                _rotationTravelX = 0;
                _rotationTravelY = 0;
                //Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // Placement target
            var targetVoxel = Vector3Int.zero;
            var targetPosition = VoxelSpace.GetWorldPosition(targetVoxel);


            // Scroll wheel
            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll < 0)
                _yPos += 1;
            else if (scroll > 0)
                _yPos -= 1;
            
            _yPos = Mathf.Min(_yPos, Room.Instance.FillVolume.MaxBounds.y);
            _yPos = Mathf.Max(_yPos, Room.Instance.FillVolume.MinBounds.y);

            if(EventSystem.current.IsPointerOverGameObject())
                return;

            // Raycasts to floor
            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            var distance = 0f;
            if (_plane.Raycast(cameraRay, out distance))
            {
                // Raycast to floor
                var point = cameraRay.GetPoint(distance);
                Debug.DrawLine(Camera.main.transform.position, point, Color.magenta, 0.1f);

                // Clip to room bounds
                point = Room.Instance.Collider.bounds.ClosestPoint(point);

                targetVoxel = VoxelSpace.GetWorldVoxelFromWorldPos(point);
                targetVoxel.y = _yPos;
                targetPosition = VoxelSpace.GetWorldPosition(targetVoxel);
            }
            
            // Get collisions in TARGET location
            var collisions = VoxelSpace.InverseCollistionsA(targetPosition, _target.Volume,
                Room.Instance.transform.position, Room.Instance.FillVolume);

            var collisionsWithObjects = VoxelSpace.CollistionsA(targetPosition, _target.Volume,
                Room.Instance.transform.position, Room.Instance.ObjectsVolume);
            collisions.AddRange(collisionsWithObjects);
            
            // Draw red voxels
            foreach (var collision in collisions)
            {
                //var pos = _target.VoxelObject.GetActualWorldVoxel(collision.LocalPosA);
                var pos = VoxelSpace.GetWorldPosition(collision.WorldPos);
                Graphics.DrawMesh(_voxelMesh, Matrix4x4.Translate(pos), RedVoxel, layer: LayerMask.GetMask("Default"));
            }

            // If we are allowed to actaully move the object
            if (!_placeLock && !_isRotating)
            {
                _target.MoveSmooth(targetPosition);

                if (Input.GetButtonDown("Place"))
                {
                    // If no collisions in target find fall distance
                    if (collisions.Count == 0)
                    {
                        var fallDistance = GetFallDistance();
                        targetVoxel.y -= fallDistance;
                        targetPosition = VoxelSpace.GetWorldPosition(targetVoxel);

                        _placeLock = true;
                        _target.MoveSmooth(targetPosition, () =>
                        {
                            Room.Instance.PlaceFurniture(_target);
                            _target = null;
                            GetNextTarget();
                            _placeLock = false;
                            SoundManager.Instance.Play(PlaceSound);
                        });
                    }
                    else
                    {
                        SoundManager.Instance.Play(CantPlaceSound);
                        Debug.Log("Can't place");
                    }
                }
            }
        }

        private int GetFallDistance()
        {
            var fallDistance = 100;
            var objectsProfile = Room.Instance.ObjectsVolume.GetHighProfile();
            var furnitureLowProfile = _target.Volume.GetLowProfile();
            foreach (var kvp in furnitureLowProfile)
            {
                var voxelFurniture = new Vector3Int(kvp.Key.x, kvp.Value, kvp.Key.y);
                var voxelWorldFuniture = VoxelSpace.GetWorldVoxelFromLocalVoxel(_target.transform.position,
                    voxelFurniture - _target.Volume.Pivot);

                var voxelLocalRoom = VoxelSpace.GetLocalVoxelFromWorldVoxel(Room.Instance.transform.position,
                    voxelWorldFuniture + Room.Instance.ObjectsVolume.Pivot);
                var roomXz = new Vector2Int(voxelLocalRoom.x, voxelLocalRoom.z);
                if (objectsProfile.ContainsKey(roomXz))
                {
                    Debug.DrawRay(VoxelSpace.GetWorldPositionCenter(voxelWorldFuniture), Vector3.up, Color.red, 1f);
                    var localH = objectsProfile[roomXz];
                    var voxelWorldRoom = VoxelSpace.GetWorldVoxelFromLocalVoxel(Room.Instance.transform.position,
                        new Vector3Int(roomXz.x, localH, roomXz.y) - Room.Instance.ObjectsVolume.Pivot);

                    fallDistance = Mathf.Min(Mathf.Abs(voxelWorldRoom.y - voxelWorldFuniture.y) - 1, fallDistance);
                    fallDistance = Mathf.Max(fallDistance, 0);
                }

                fallDistance = Mathf.Min(voxelWorldFuniture.y, fallDistance);
            }

            if (fallDistance == 100)
                return 0;
            return fallDistance;
        }
    }
}
