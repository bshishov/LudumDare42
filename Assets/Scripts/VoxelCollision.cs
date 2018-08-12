using UnityEngine;

namespace Assets.Scripts
{
    public struct VoxelCollision
    {
        public Vector3Int LocalPosA;
        public Vector3Int LocalPosB;
        public Vector3Int WorldPos;
        public VoxelData DataA;
        public VoxelData DataB;
    }
}
