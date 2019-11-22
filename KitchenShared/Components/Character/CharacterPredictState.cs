using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    public struct CharacterPredictState : IComponentData,IPredict<CharacterPredictState>
    {
        public float3 position;
        public quaternion rotation;
        public Entity pickupEntity;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            position = reader.ReadVector3Q();
            rotation = reader.ReadQuaternionQ();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("position", position);
            writer.WriteQuaternionQ("rotation", rotation);
        }

        public bool VerifyPrediction(ref CharacterPredictState state)
        {
            return position.Equals(state.position) &&
                rotation.Equals(state.rotation) &&
                pickupEntity.Equals(state.pickupEntity);
        }
    }

}