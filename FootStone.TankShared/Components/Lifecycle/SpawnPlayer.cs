using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [InternalBufferCapacity(10)]
    public struct PlayerId : IBufferElementData
    {
        public int playerId;
    }

    [Serializable]
    public struct SpawnPlayer : IComponentData
    {
        public Entity entity;
        public bool spawned;
    }

}