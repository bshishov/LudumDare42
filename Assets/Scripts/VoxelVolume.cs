using System;
using System.Collections;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    public enum VoxelType
    {
        Default,
    }

    [Serializable]
    public class VoxelData
    {
        [SerializeField]
        public VoxelType Type;
    }

    [Serializable]
    public class VoxelDataDict : SerializableDictionary<Vector3Int, VoxelData> { }

    [Serializable]
    public class VoxelVolume
    {
        public Vector3Int Pivot;

        [SerializeField]
        public VoxelDataDict Data;

        public VoxelData GetDataAt(Vector3Int localPosition)
        {
            VoxelData data;
            if (Data.TryGetValue(localPosition, out data))
                return data;

            return null;
        }

        public bool HasVoxelAt(Vector3Int localPosition)
        {
            return Data.ContainsKey(localPosition);
        }

        public void RotateYCw()
        {
            // (x, y, z) -> (z, y, -x)
            var newData = new VoxelDataDict();
            foreach (var kvp in Data)
            {
                var p = kvp.Key;
                var newP = new Vector3Int(p.z, p.y, -p.x);
                newData.Add(newP, kvp.Value);
            }
            Data = newData;
            Pivot = new Vector3Int(Pivot.z, Pivot.y, -Pivot.x);
        }

        public void RotateXCw()
        {
            // (x, y, z) -> (x, -z, y)
            var newData = new VoxelDataDict();
            foreach (var kvp in Data)
            {
                var p = kvp.Key;
                var newP = new Vector3Int(p.x, -p.z, p.y);
                newData.Add(newP, kvp.Value);
            }
            Data = newData;
            Pivot = new Vector3Int(Pivot.x, -Pivot.z, Pivot.y);
        }

        public IEnumerator EnumerateIntersections(VoxelVolume targetVolume)
        {
            yield return null;
        }

        public IEnumerator EnumerateInverseIntersections(VoxelVolume targetVolume)
        {
            yield return null;
        }

        public void SetAt(Vector3Int localPosition, VoxelData data)
        {
            if (Data.ContainsKey(localPosition))
                Data[localPosition] = data;
            else
            {
                Data.Add(localPosition, data);
                RecalculateBounds();
            }
        }

        public void RemoveAt(Vector3Int localPosition)
        {
            if (Data.ContainsKey(localPosition))
                Data.Remove(localPosition);
        }

        public void RecalculateBounds()
        {
        }
    }
}
