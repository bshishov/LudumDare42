using System;
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts
{
    [Serializable]
    public class RoomTask
    {
        [SerializeField]
        public Furniture Furniture;

        [SerializeField]
        public int Amount;
    }
}