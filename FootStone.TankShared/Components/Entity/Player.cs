using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
  
    public struct Player : IComponentData
    {
        public int id;
        public int playerId;       
    }
}

