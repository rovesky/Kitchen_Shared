using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct ItemPredictedState : IComponentData, IPredictedState<ItemPredictedState>
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

        public bool VerifyPrediction(ref ItemPredictedState state)
        {
            return Position.Equals(state.Position) &&
                   Rotation.Equals(state.Rotation) &&
                   Owner.Equals(state.Owner);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<ItemPredictedState>();
        }
    }
}