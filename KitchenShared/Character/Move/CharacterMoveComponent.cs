using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

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
        public float3 InitVelocity;
        public ushort VelocityDulation;

      //  public CharacterSupportState SupportedState;
      //  public bool IsJumping;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            UnsupportedVelocity = reader.ReadVector3Q();
            InitVelocity = reader.ReadVector3Q();

            //  SupportedState = (CharacterSupportState)reader.ReadByte();
            //  IsJumping = reader.ReadBoolean();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("UnsupportedVelocity", UnsupportedVelocity);
            writer.WriteVector3Q("InitVelocity", InitVelocity);

            //writer.WriteByte("SupportedState", (byte)SupportedState);
            //writer.WriteBoolean("IsJumping", IsJumping);
        }

        public bool VerifyPrediction(ref CharacterMovePredictedState state)
        {
            return UnsupportedVelocity.Equals(state.UnsupportedVelocity) &&
                   InitVelocity.Equals(state.InitVelocity);
            // IsJumping.Equals(state.IsJumping);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<CharacterMovePredictedState>();
        }
    }
}
