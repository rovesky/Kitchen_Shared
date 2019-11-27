using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    public struct CharacterPredictState : IComponentData,IPredict<CharacterPredictState>
    {
        public float3 Position;
        public quaternion Rotation;
        public Entity PickupedEntity;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Position = reader.ReadVector3Q();
            Rotation = reader.ReadQuaternionQ();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("Position", Position);
            writer.WriteQuaternionQ("rotation", Rotation);
        }

        public bool VerifyPrediction(ref CharacterPredictState state)
        {
            return Position.Equals(state.Position) &&
                Rotation.Equals(state.Rotation) &&
                PickupedEntity.Equals(state.PickupedEntity);
        }
    }

}