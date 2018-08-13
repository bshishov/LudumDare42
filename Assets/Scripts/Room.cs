using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    [Serializable]
    public class FurnitureConstraint
    {
        public Furniture A;
        public Furniture B;
    }

    [RequireComponent(typeof(VoxelObject))]
    public class Room : Singleton<Room>
    {
        public VoxelVolume FillVolume {get { return _voxelObject.Volume; } }
        public BoxCollider Collider { get { return _collider; } }
        public VoxelVolume ObjectsVolume
        {
            get
            {
                if (ObjectsVoxelObject == null)
                    return null;
                return ObjectsVoxelObject.Volume;
            }
        }

        [TextArea]
        public string Text;

        [TextArea]
        public string FinishText;

        public List<RoomTask> FurniturePool = new List<RoomTask>();
        public List<RoomTask> FurnitureTasks = new List<RoomTask>();
        public List<FurnitureConstraint> FurnitureConstraints = new List<FurnitureConstraint>();
        public string NextScene;

        private readonly VoxelVolume _objectsVolume = new VoxelVolume();
        private VoxelObject _voxelObject;
        private readonly List<VoxelFurniture> _placedFurniture = new List<VoxelFurniture>();

        [Header("Gameplay")]
        public Stack<Furniture> FurnitureStack = new Stack<Furniture>();

        [Header("System")]
        public VoxelObject ObjectsVoxelObject;

        private BoxCollider _collider;

        private void Awake()
        {
            _voxelObject = GetComponent<VoxelObject>();

            // Generate tasks
            var allFurniture = new List<Furniture>();
            foreach (var furnitureTask in FurniturePool)
            {
                for (var i = 0; i < furnitureTask.Amount; i++)
                    allFurniture.Add(furnitureTask.Furniture);
            }

            allFurniture = allFurniture.OrderBy(x => Random.value).ToList();
            foreach (var f in allFurniture)
                FurnitureStack.Push(f);
        }

        private void Start()
        {
            _collider = _voxelObject.UpdateCollider();
        }

        private void Update()
        {
        }

        public void PlaceFurniture(VoxelFurniture furniture)
        {
            //furniture.FinishMovement();
            foreach (var kvp in furniture.Volume.Data)
            {
                var worldVoxel = VoxelSpace.GetWorldVoxelFromLocalVoxel(furniture.transform.position,
                    kvp.Key - furniture.Volume.Pivot);
                var localVoxel =
                    VoxelSpace.GetLocalVoxelFromWorldVoxel(transform.position, worldVoxel + ObjectsVolume.Pivot);
                ObjectsVolume.SetAt(localVoxel, kvp.Value);
            }

            _placedFurniture.Add(furniture);
        }

        public bool Star1()
        {
            foreach (var task in FurnitureTasks)
            {
                var placed = _placedFurniture.Count(f => f.Furniture.Equals(task.Furniture));
                if (placed < task.Amount)
                    return false;
            }

            return true;
        }

        public bool Star2()
        {
            var totalRequired = FurniturePool.Sum(task => task.Amount);
            var placed = _placedFurniture.Count;
            return placed >= totalRequired;
        }

        public bool Star3()
        {
            if (FurnitureConstraints.Count == 0)
                return true;

            return FurnitureConstraints.All(constraint => CheckConstraint(constraint.A, constraint.B));
        }

        public bool CheckConstraint(Furniture a, Furniture b)
        {
            var aObjects = _placedFurniture.Where(f => f.Furniture.Equals(a)).ToList();
            var bObjects = _placedFurniture.Where(f => f.Furniture.Equals(b)).ToList();

            if (!aObjects.Any() || !bObjects.Any())
                return false;

            return aObjects.Any(oa => bObjects.Any(ob => CheckNear(oa, ob)));
        }

        public bool CheckNear(VoxelFurniture a, VoxelFurniture b)
        {
            var bA = a.VoxelObject.GetWorldBounds();
            var bB = a.VoxelObject.GetWorldBounds();

            bA.size *= 1.1f;
            return bA.Intersects(bB);
        }
    }
}
