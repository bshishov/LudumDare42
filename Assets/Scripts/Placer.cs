using Assets.Scripts.UI;
using Assets.Scripts.Utils;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class Placer : Singleton<Placer>
    {
        private VoxelFurniture _target;

        [Header("Gameplay")]
        public float Timer = 10f;

        [Header("Visuals")]
        public Material RedVoxel;

        private Plane _plane = new Plane(Vector3.up, 0f);
        private int _yPos = 0;
        private Mesh _voxelMesh;
        private bool _placeLock;
        private float _remainingTime;

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
                    var go = GameObject.Instantiate(furniture.Prefab, new Vector3(0, 2, 0), Quaternion.identity);
                    _target = go.GetComponent<VoxelFurniture>();
                    _target.Furniture = furniture;

                    _remainingTime = Timer;
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

            _remainingTime -= Time.deltaTime;
            if (_remainingTime < 0)
            {
                _placeLock = true;
                _target.MoveSmooth(new Vector3(0, 5, 0), () =>
                {
                    Destroy(_target.gameObject);
                    GetNextTarget();
                    _placeLock = false;
                });
                
                return;
            }

            // Rotation
            if (Input.GetButtonDown("RotateY"))
                _target.RotateYCw();

            if (Input.GetButtonDown("RotateX"))
                _target.RotateXCw();

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
            if (!_placeLock)
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
                        });
                    }
                    else
                    {
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
