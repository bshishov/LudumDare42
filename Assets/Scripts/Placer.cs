using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public class Placer : Singleton<Placer>
    {
        private VoxelFurniture _target;

        [Header("Visuals")] public Material RedVoxel;

        private Plane _plane = new Plane(Vector3.up, 0f);
        private int _yPos = 0;
        private Mesh _voxelMesh;

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
                    var go = GameObject.Instantiate(furniture.Prefab, Vector3.zero, Quaternion.identity);
                    _target = go.GetComponent<VoxelFurniture>();
                }
            }
        }

        private void Update()
        {
            if (_target == null || Room.Instance == null)
                return;

            var collisions = VoxelSpace.InverseCollistionsA(_target.transform.position, _target.Volume,
                Room.Instance.transform.position, Room.Instance.FillVolume);

            var collisionsWithObjects = VoxelSpace.CollistionsA(_target.transform.position, _target.Volume,
                Room.Instance.transform.position, Room.Instance.ObjectsVolume);
            collisions.AddRange(collisionsWithObjects);
            
            foreach (var collision in collisions)
            {
                var pos = _target.VoxelObject.GetActualWorldVoxel(collision.LocalPosA);
                //var pos = VoxelSpace.GetWorldPosition(collision.WorldPos);
                Graphics.DrawMesh(_voxelMesh, Matrix4x4.Translate(pos), RedVoxel, layer: LayerMask.GetMask("Default"));
            }

            var scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll < 0)
                _yPos += 1;
            else if(scroll > 0)
                _yPos -= 1;

            if (Input.GetKeyDown(KeyCode.R) || Input.GetButtonDown("Fire2"))
                _target.RotateYCw();

            if (Input.GetKeyDown(KeyCode.F))
                _target.RotateXCw();

            var cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            var distance = 0f;
            if (_plane.Raycast(cameraRay, out distance))
            {
                var point = cameraRay.GetPoint(distance);
                Debug.DrawLine(Camera.main.transform.position, point, Color.magenta, 0.1f);

                //if (VoxelSpace.IsInVoxelVolume(point, Room.Instance.transform.position, Room.Instance.Volume))
                {
                    var voxelPos = VoxelSpace.GetWorldVoxelFromWorldPos(point);
                    voxelPos.y = _yPos;

                    var targetPos = VoxelSpace.GetWorldPosition(voxelPos);
                    _target.MoveSmooth(targetPos);
                }
            }

            if (Input.GetButtonDown("Fire1"))
            {
                if (collisions.Count == 0)
                {
                    Room.Instance.PlaceFurniture(_target);
                    _target = null;
                    GetNextTarget();
                }
                else
                {
                    Debug.Log("Can't place");
                }
            }
        }
    }
}
