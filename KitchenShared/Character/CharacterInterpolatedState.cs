using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    public struct CharacterInterpolatedState : IComponentData, IInterpolatedState<CharacterInterpolatedState>
    {
        public float3 Position;
        public quaternion Rotation;
   
        public float Velocity;
        public bool IsTake;
        public bool IsSlice;
        public bool IsClean;
        public bool IsThrow;


        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Position = reader.ReadVector3Q();
            Rotation = reader.ReadQuaternionQ();
            Velocity = reader.ReadFloatQ();
            IsTake = reader.ReadBoolean();
            IsSlice = reader.ReadBoolean();
            IsClean = reader.ReadBoolean();
            IsThrow = reader.ReadBoolean();

        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("position", Position);
            writer.WriteQuaternionQ("rotation", Rotation);
            writer.WriteFloatQ("Velocity", Velocity);
            writer.WriteBoolean("IsTake",IsTake);
            writer.WriteBoolean("IsSlice",IsSlice);
            writer.WriteBoolean("IsClean",IsClean);
            writer.WriteBoolean("IsThrow",IsThrow);

        }

        public void Interpolate(ref SerializeContext context, ref CharacterInterpolatedState prevState,
            ref CharacterInterpolatedState nextState, float interpVal)
        {
            Position = Vector3.Lerp(prevState.Position, nextState.Position, interpVal);
            Rotation = Quaternion.Lerp(prevState.Rotation, nextState.Rotation, interpVal);
            Velocity = math.lerp(prevState.Velocity, nextState.Velocity, interpVal);
            IsTake = prevState.IsTake;
            IsSlice = prevState.IsSlice;  
            IsClean = prevState.IsClean;
            IsThrow = prevState.IsThrow;
        }

        public static IInterpolatedStateSerializerFactory CreateSerializerFactory()
        {
            return new InterpolatedStateSerializerFactory<CharacterInterpolatedState>();
        }
    }
}