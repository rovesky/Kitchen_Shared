using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [Serializable]
    public struct EntityPredictData : IComponentData
    {
        public float3 position;
        public quaternion rotation;

        public override bool Equals(object obj)
        {
            var minValue = 0.001;
            var other = (EntityPredictData)obj;
            return Mathf.Abs(this.position.x - other.position.x) < minValue &&
                   Mathf.Abs(this.position.y - other.position.y) < minValue &&
                   Mathf.Abs(this.position.z - other.position.z) < minValue;

        }
    }

}