using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace FootStone.Kitchen
{
    public struct ItemPredictedState : IComponentData, IPredictedState<ItemPredictedState>
    {
        public float3     Position;
        public quaternion Rotation;
        public float3     Velocity;
        public PhysicsMass Mass;
        public Entity     Owner;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Position = reader.ReadVector3Q();
            Rotation = reader.ReadQuaternionQ();
            Velocity = reader.ReadVector3Q();
            context.RefSerializer.DeserializeReference(ref reader, ref Owner);
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("position", Position);
            writer.WriteQuaternionQ("rotation", Rotation);
            writer.WriteVector3Q("velocity", Velocity);
            context.RefSerializer.SerializeReference(ref writer, "owner", Owner);
        }

        public bool VerifyPrediction(ref ItemPredictedState state)
        {
            return Position.Equals(state.Position) &&
                   Rotation.Equals(state.Rotation) &&
                   Velocity.Equals(state.Velocity) &&
                   Owner.Equals(state.Owner);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<ItemPredictedState>();
        }
    }
}