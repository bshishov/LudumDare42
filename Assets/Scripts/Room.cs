using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Data;
using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts
{
    [RequireComponent(typeof(VoxelObject))]
    public class Room : Singleton<Room>
    {
        public VoxelVolume FillVolume {get { return _voxelObject.Volume; } }

        public VoxelVolume ObjectsVolume
        {
            get
            {
                if (ObjectsVoxelObject == null)
                    return null;
                return ObjectsVoxelObject.Volume;
            }
        }

        public List<RoomTask> FurnitureTasks = new List<RoomTask>();

        private readonly VoxelVolume _objectsVolume = new VoxelVolume();
        private VoxelObject _voxelObject;

        [Header("Gameplay")]
        public Stack<Furniture> FurnitureStack = new Stack<Furniture>();

        [Header("System")]
        public VoxelObject ObjectsVoxelObject;

        private void Start()
        {
            _voxelObject = GetComponent<VoxelObject>();

            // Generate tasks
            var allFurniture = new List<Furniture>();
            foreach (var furnitureTask in FurnitureTasks)
            {
                for (var i = 0; i < furnitureTask.Amount; i++)
                    allFurniture.Add(furnitureTask.Furniture);
            }

            allFurniture = allFurniture.OrderBy(x => Random.value).ToList();
            foreach (var f in allFurniture)
                FurnitureStack.Push(f);
        }

        private void Update()
        {
        }

        public void PlaceFurniture(VoxelFurniture furniture)
        {
            foreach (var kvp in furniture.Volume.Data)
            {
                var worldVoxel = VoxelSpace.GetWorldVoxelFromLocalVoxel(furniture.transform.position,
                    kvp.Key - furniture.Volume.Pivot);
                var localVoxel =
                    VoxelSpace.GetLocalVoxelFromWorldVoxel(transform.position, worldVoxel + ObjectsVolume.Pivot);
                ObjectsVolume.SetAt(localVoxel, kvp.Value);
            }
        }
    }
}
