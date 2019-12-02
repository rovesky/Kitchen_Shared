using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{

    public struct ItemInterpolatedState : IComponentData, IInterpolatedState<ItemInterpolatedState>
    {
        public float3 Position;
        public quaternion Rotation;
        public Entity Owner;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Position = reader.ReadVector3Q();
            Rotation = reader.ReadQuaternionQ();
            context.RefSerializer.DeserializeReference(ref reader, ref Owner);
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("position", Position);
            writer.WriteQuaternionQ("rotation", Rotation);
            context.RefSerializer.SerializeReference(ref writer, "owner", Owner);
        }

        public void Interpolate(ref SerializeContext context, ref ItemInterpolatedState prevState, ref ItemInterpolatedState nextState, float interpVal)
        {
            if (prevState.Owner == nextState.Owner)
            {
                Position = Vector3.Lerp(prevState.Position, nextState.Position, interpVal);
                Rotation = Quaternion.Lerp(prevState.Rotation, nextState.Rotation, interpVal);
                Owner = prevState.Owner;
            }
            else
            {
                Position = prevState.Position;
                Rotation = prevState.Rotation;
                Owner = prevState.Owner;
            }
        }

        public static IInterpolatedStateSerializerFactory CreateSerializerFactory()
        {
            return new InterpolatedStateSerializerFactory<ItemInterpolatedState>();
        }
    }
}
