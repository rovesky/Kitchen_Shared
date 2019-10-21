using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Assets.Scripts.ECS
{

    [Serializable]
    public struct EntityPredictData : IComponentData
    {
        public float3 position;
        public quaternion rotation;
    }

}