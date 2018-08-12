using UnityEngine;

namespace Assets.Scripts
{
    public class VoxelObject : MonoBehaviour
    {
        public VoxelVolume Volume;

        [Header("Debug")]
        public bool DrawGizmos;
        public Color CellColor = Color.blue;

        [ContextMenu("Rotate Y CW")]
        public void RotateYCw()
        {
            Volume.RotateYCw();
        }

        [ContextMenu("Rotate X CW")]
        public void RotateXCw()
        {
            Volume.RotateXCw();
        }

        public Vector3 GetPivotCenter()
        {
            return (Vector3)Volume.Pivot * VoxelSpace.VoxelSize + transform.position + VoxelSpace.HalfVoxelSize3D;

            //var pivotVoxel = VoxelSpace.GetWorldVoxelFromLocalVoxel(transform.position, Volume.Pivot);
            //return VoxelSpace.GetWorldPositionCenter(pivotVoxel);
        }

        [ContextMenu("Populate")]
        public void DebugPopulate()
        {
            Volume.Pivot = new Vector3Int(0, 0, 2);
            Volume.Data = new VoxelDataDict();
            Volume.SetAt(new Vector3Int(0, 0, 0), new VoxelData());
            Volume.SetAt(new Vector3Int(0, 0, 1), new VoxelData());
            Volume.SetAt(new Vector3Int(0, 0, 2), new VoxelData());
            Volume.SetAt(new Vector3Int(0, 0, 3), new VoxelData());
            Volume.SetAt(new Vector3Int(1, 0, 3), new VoxelData());
            Volume.SetAt(new Vector3Int(0, 1, 3), new VoxelData());
        }

        private void OnDrawGizmos()
        {
            if (!DrawGizmos)
                return;

            const float Opacity = 0.5f;

            // Voxels
            //Gizmos.color = new Color(1f, 1f, 1f, Opacity);
            Gizmos.color = CellColor;
            foreach (var kvp in Volume.Data)
            {
                var pos = VoxelSpace.GetWorldPositionCenter(VoxelSpace.GetWorldVoxelFromLocalVoxel(transform.position, kvp.Key - Volume.Pivot));
                Gizmos.DrawWireCube(pos, VoxelSpace.VoxelSize3D);
            }

            // Pivot
            Gizmos.color = new Color(1f, 0f, 0f, Opacity);
            var pivotPos = VoxelSpace.GetWorldPositionCenter(VoxelSpace.GetWorldVoxelFromLocalVoxel(transform.position, Volume.Pivot - Volume.Pivot));
            Gizmos.DrawCube(pivotPos, VoxelSpace.VoxelSize3D * 0.9f);
        }

        public Vector3 GetActualWorldVoxel(Vector3Int voxel)
        {
            var localVoxel = voxel - Volume.Pivot;
            return transform.position + (Vector3)localVoxel * VoxelSpace.VoxelSize;
        }

        public BoxCollider UpdateCollider()
        {
            var col = GetComponent<BoxCollider>();

            if(col == null)
                col = gameObject.AddComponent<BoxCollider>();

            Volume.RecalculateBounds();
            var minBounds = VoxelSpace.GetWorldPositionCenter(VoxelSpace.GetWorldVoxelFromLocalVoxel(transform.position, Volume.MinBounds - Volume.Pivot));
            var maxBounds = VoxelSpace.GetWorldPositionCenter(VoxelSpace.GetWorldVoxelFromLocalVoxel(transform.position, Volume.MaxBounds - Volume.Pivot));
            var boundsCenter = Vector3.Lerp(minBounds, maxBounds, 0.5f);
            var boundSize = new Vector3(
                Mathf.Abs(maxBounds.x - minBounds.x),
                Mathf.Abs(maxBounds.y - minBounds.y),
                Mathf.Abs(maxBounds.z - minBounds.z)
            );
            col.center = transform.InverseTransformPoint(boundsCenter);
            col.size = boundSize + VoxelSpace.VoxelSize3D;

            return col;
        }
    }
}
