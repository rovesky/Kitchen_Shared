using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
	public struct CharacterMoveSetting : IComponentData
	{
        public float3 Gravity;
        public float Velocity;
        public float MaxVelocity;
        public float RotationVelocity;
        public float JumpUpwardsVelocity;
        public float MaxSlope; // radians
        public int MaxIterations;
        public float CharacterMass;
        public float SkinWidth;
        public float ContactTolerance;
        public int AffectsPhysicsBodies;
    }

    public enum CharacterSupportState : byte
    {
        Unsupported = 0,
        Sliding,
        Supported
    }

    public struct CharacterMovePredictedState : IComponentData, IPredictedState<CharacterMovePredictedState>
    {
        public float3 UnsupportedVelocity;
        public float3 ImpulseVelocity;
        public ushort ImpulseDuration;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            UnsupportedVelocity = reader.ReadVector3Q();
            ImpulseVelocity = reader.ReadVector3Q();
            ImpulseDuration = reader.ReadUInt16();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("UnsupportedVelocity", UnsupportedVelocity);
            writer.WriteVector3Q("ImpulseVelocity", ImpulseVelocity);
            writer.WriteUInt16("ImpulseDuration",ImpulseDuration);
           
        }

        public bool VerifyPrediction(ref CharacterMovePredictedState state)
        {
            return Vector3.SqrMagnitude(UnsupportedVelocity - state.UnsupportedVelocity) < 0.001f &&
                   Vector3.SqrMagnitude(ImpulseVelocity - state.ImpulseVelocity) < 0.001f &&
                   ImpulseDuration.Equals(state.ImpulseDuration);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<CharacterMovePredictedState>();
        }
    }
}
