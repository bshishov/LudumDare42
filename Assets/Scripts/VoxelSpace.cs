using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class VoxelSpace
    {
        public static float VoxelSize = 0.5f;
        public static Vector3 VoxelSize3D = new Vector3(0.5f, 0.5f, 0.5f);
        public static Vector3 HalfVoxelSize3D = VoxelSize3D * 0.5f;

        public static Vector3Int GetWorldVoxelFromLocalVoxel(Vector3 objPosition, Vector3Int localVoxel)
        {
            return GetWorldVoxelFromWorldPos(objPosition) + localVoxel;
        }

        public static Vector3Int GetWorldVoxelFromWorldPos(Vector3 position)
        {
            return new Vector3Int(
                Mathf.RoundToInt(position.x / VoxelSize),
                Mathf.RoundToInt(position.y / VoxelSize),
                Mathf.RoundToInt(position.z / VoxelSize)
            );
        }

        public static Vector3Int GetLocalVoxelFromWorldVoxel(Vector3 objPosition, Vector3Int worldWorxel)
        {
            return worldWorxel - GetWorldVoxelFromWorldPos(objPosition);
        }

        public static Vector3 GetWorldPosition(Vector3Int worldVoxelPosition)
        {
            return new Vector3(
                worldVoxelPosition.x * VoxelSize,
                worldVoxelPosition.y * VoxelSize,
                worldVoxelPosition.z * VoxelSize
            );
        }

        public static Vector3 GetWorldPositionCenter(Vector3Int worldVoxelPosition)
        {
            return new Vector3(
                worldVoxelPosition.x * VoxelSize,
                worldVoxelPosition.y * VoxelSize,
                worldVoxelPosition.z * VoxelSize
            ) + HalfVoxelSize3D;
        }

        public static Vector3 SnapPosition(Vector3 worldPos)
        {
            var voxelPos = GetWorldVoxelFromWorldPos(worldPos);
            return GetWorldPosition(voxelPos);
        }

        public static List<VoxelCollision> CollistionsA(Vector3 positionA, VoxelVolume volumeA, Vector3 positionB, VoxelVolume volumeB)
        {
            var collisions = new List<VoxelCollision>();
            foreach (var kvpA in volumeA.Data)
            {
                var worldVoxelA = GetWorldVoxelFromLocalVoxel(positionA, kvpA.Key - volumeA.Pivot);
                var localVoxelB = GetLocalVoxelFromWorldVoxel(positionB, worldVoxelA + volumeB.Pivot);
                if (volumeB.HasVoxelAt(localVoxelB))
                    collisions.Add(new VoxelCollision
                    {
                        WorldPos = worldVoxelA,
                        LocalPosA = kvpA.Key,
                        LocalPosB = localVoxelB
                    });
            }
            return collisions;
        }

        public static List<VoxelCollision> InverseCollistionsA(Vector3 positionA, VoxelVolume volumeA, Vector3 positionB, VoxelVolume volumeB)
        {
            var collisions = new List<VoxelCollision>();
            foreach (var kvpA in volumeA.Data)
            {
                var worldVoxelA = GetWorldVoxelFromLocalVoxel(positionA, kvpA.Key - volumeA.Pivot);
                var localVoxelB = GetLocalVoxelFromWorldVoxel(positionB, worldVoxelA + volumeB.Pivot);
                if (!volumeB.HasVoxelAt(localVoxelB))
                    collisions.Add(new VoxelCollision
                    {
                        WorldPos = worldVoxelA,
                        LocalPosA = kvpA.Key,
                        LocalPosB = localVoxelB
                    });
            }
            return collisions;
        }

        public static bool IsInVoxelVolume(Vector3 point, Vector3 objPosition, VoxelVolume volume)
        {
            var voxelPos = GetWorldVoxelFromWorldPos(point);
            return volume.HasVoxelAt(GetLocalVoxelFromWorldVoxel(objPosition, voxelPos + volume.Pivot));
        }

        public static Mesh CreateVoxelMesh()
        {
            var mesh = new Mesh();
            var d = VoxelSize * 1.1f;
            Vector3[] vertices = {
                new Vector3 (0, 0, 0),
                new Vector3 (d, 0, 0),
                new Vector3 (d, d, 0),
                new Vector3 (0, d, 0),
                new Vector3 (0, d, d),
                new Vector3 (d, d, d),
                new Vector3 (d, 0, d),
                new Vector3 (0, 0, d),
            };

            int[] triangles = {
                0, 2, 1, //face front
                0, 3, 2,
                2, 3, 4, //face top
                2, 4, 5,
                1, 2, 5, //face right
                1, 5, 6,
                0, 7, 4, //face left
                0, 4, 3,
                5, 4, 7, //face back
                5, 7, 6,
                0, 6, 7, //face bottom
                0, 1, 6
            };
            
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}
