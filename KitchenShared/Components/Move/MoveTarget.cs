using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [Serializable]
    public struct MoveTarget : IComponentData
    {
        public float Speed;      
    }

}