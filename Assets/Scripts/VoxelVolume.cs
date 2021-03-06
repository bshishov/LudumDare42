﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
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

        public Vector3Int MinBounds;
        public Vector3Int MaxBounds;

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

        public void RotateYCw(bool updateBounds = false)
        {
            // (x, y, z) -> (z, y, -x)
            Rotate(new Vector3Int(0, 0, 1), Vector3Int.up, Vector3Int.left);

            /*
            var newData = new VoxelDataDict();
            foreach (var kvp in Data)
            {
                var p = kvp.Key;
                var newP = new Vector3Int(p.z, p.y, -p.x);
                newData.Add(newP, kvp.Value);
            }
            Data = newData;
            Pivot = new Vector3Int(Pivot.z, Pivot.y, -Pivot.x);

            if (updateBounds)
                RecalculateBounds();
                */
        }

        public void RotateYCCw(bool updateBounds = false)
        {
            // (x, y, z) -> (-z, y, x)
            Rotate(new Vector3Int(0, 0, -1), Vector3Int.up, Vector3Int.right);
        }

        public void RotateXCw(bool updateBounds=false)
        {
            // (x, y, z) -> (x, -z, y)
            Rotate(Vector3Int.right, new Vector3Int(0, 0, -1), Vector3Int.up);
        }

        public void RotateXCCw(bool updateBounds = false)
        {
            // (x, y, z) -> (x, -z, y)
            Rotate(Vector3Int.right, new Vector3Int(0, 0, 1), Vector3Int.down);
        }

        private static int Multiply(Vector3Int p, Vector3Int w)
        {
            return p.x * w.x + p.y * w.y + p.z * w.z;
        }

        public void Rotate(Vector3Int xw, Vector3Int yw, Vector3Int zw, bool updateBounds = false)
        {
            var newData = new VoxelDataDict();
            foreach (var kvp in Data)
            {
                var p = kvp.Key;

                var newP = new Vector3Int(Multiply(p, xw), Multiply(p, yw), Multiply(p, zw));
                newData.Add(newP, kvp.Value);
            }
            Data = newData;
            Pivot = new Vector3Int(Multiply(Pivot, xw), Multiply(Pivot, yw), Multiply(Pivot, zw));

            if (updateBounds)
                RecalculateBounds();
        }

        public void SetAt(Vector3Int localPosition, VoxelData data, bool updateBounds=false)
        {
            if (Data.ContainsKey(localPosition))
                Data[localPosition] = data;
            else
            {
                Data.Add(localPosition, data);

                if (updateBounds)
                {
                    MaxBounds.x = Math.Max(localPosition.x, MaxBounds.x);
                    MaxBounds.y = Math.Max(localPosition.y, MaxBounds.y);
                    MaxBounds.z = Math.Max(localPosition.z, MaxBounds.z);

                    MinBounds.x = Math.Min(localPosition.x, MinBounds.x);
                    MinBounds.y = Math.Min(localPosition.y, MinBounds.y);
                    MinBounds.z = Math.Min(localPosition.z, MinBounds.z);
                }
            }
        }

        public void RemoveAt(Vector3Int localPosition, bool updateBounds = false)
        {
            if (Data.ContainsKey(localPosition))
            {
                Data.Remove(localPosition);

                if(updateBounds)
                    RecalculateBounds();
            }
        }

        public void RecalculateBounds()
        {
            MaxBounds = new Vector3Int(-1000, -1000, -1000);
            MinBounds = new Vector3Int(1000, 1000, 1000);

            foreach (var kvp in Data)
            {
                MaxBounds.x = Math.Max(kvp.Key.x, MaxBounds.x);
                MaxBounds.y = Math.Max(kvp.Key.y, MaxBounds.y);
                MaxBounds.z = Math.Max(kvp.Key.z, MaxBounds.z);

                MinBounds.x = Math.Min(kvp.Key.x, MinBounds.x);
                MinBounds.y = Math.Min(kvp.Key.y, MinBounds.y);
                MinBounds.z = Math.Min(kvp.Key.z, MinBounds.z);
            }
        }

        public Dictionary<Vector2Int, int> GetLowProfile()
        {
            var profile = new Dictionary<Vector2Int, int>(capacity: Data.Count);
            foreach (var kvp in Data)
            {
                var xz = new Vector2Int(kvp.Key.x, kvp.Key.z);
                var y = kvp.Key.y;

                if(!profile.ContainsKey(xz))
                    profile.Add(xz, y);
                else
                {
                    if (profile[xz] > y)
                        profile[xz] = y;
                }
            }
            return profile;
        }

        public Dictionary<Vector2Int, int> GetHighProfile()
        {
            var profile = new Dictionary<Vector2Int, int>(capacity: Data.Count);
            foreach (var kvp in Data)
            {
                var xz = new Vector2Int(kvp.Key.x, kvp.Key.z);
                var y = kvp.Key.y;

                if (!profile.ContainsKey(xz))
                    profile.Add(xz, y);
                else
                {
                    if (profile[xz] < y)
                        profile[xz] = y;
                }
            }
            return profile;
        }
    }
}
