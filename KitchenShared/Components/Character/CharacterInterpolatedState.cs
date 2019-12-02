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

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Position = reader.ReadVector3Q();
            Rotation = reader.ReadQuaternionQ();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("position", Position);
            writer.WriteQuaternionQ("rotation", Rotation);
        }

        public void Interpolate(ref SerializeContext context, ref CharacterInterpolatedState prevState,
            ref CharacterInterpolatedState nextState, float interpVal)
        {
            Position = Vector3.Lerp(prevState.Position, nextState.Position, interpVal);
            Rotation = Quaternion.Lerp(prevState.Rotation, nextState.Rotation, interpVal);
        }

        public static IInterpolatedStateSerializerFactory CreateSerializerFactory()
        {
            return new InterpolatedStateSerializerFactory<CharacterInterpolatedState>();
        }
    }
}