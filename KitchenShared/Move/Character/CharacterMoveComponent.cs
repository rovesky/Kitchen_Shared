using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
	public struct CharacterMove : IComponentData
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

        public CharacterSupportState SupportedState;
        public float3 UnsupportedVelocity;
        public float3 LinearVelocity;
        public bool IsJumping;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            SupportedState = (CharacterSupportState)reader.ReadByte();
            UnsupportedVelocity = reader.ReadVector3Q();
            LinearVelocity = reader.ReadVector3Q();
            IsJumping = reader.ReadBoolean();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteByte("SupportedState", (byte)SupportedState);
            writer.WriteVector3Q("UnsupportedVelocity", UnsupportedVelocity);
            writer.WriteVector3Q("LinearVelocity", LinearVelocity);
            writer.WriteBoolean("IsJumping", IsJumping);
        }

        public bool VerifyPrediction(ref CharacterMovePredictedState state)
        {
            return SupportedState.Equals(state.SupportedState) &&
                   UnsupportedVelocity.Equals(state.UnsupportedVelocity) &&
                   LinearVelocity.Equals(state.LinearVelocity) &&
                   IsJumping.Equals(state.IsJumping);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<CharacterMovePredictedState>();
        }
    }
}
