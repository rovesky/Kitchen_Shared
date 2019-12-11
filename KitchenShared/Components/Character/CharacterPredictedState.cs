using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct CharacterPredictedState : IComponentData, IPredictedState<CharacterPredictedState>
    {
        public float3 Position;
        public quaternion Rotation;
        public Entity TriggeredEntity;
        public Entity PickupedEntity;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Position = reader.ReadVector3Q();
            Rotation = reader.ReadQuaternionQ();
            context.RefSerializer.DeserializeReference(ref reader, ref TriggeredEntity);
            context.RefSerializer.DeserializeReference(ref reader, ref PickupedEntity);
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("position", Position);
            writer.WriteQuaternionQ("rotation", Rotation);
            context.RefSerializer.SerializeReference(ref writer, "triggerEntity", TriggeredEntity);
            context.RefSerializer.SerializeReference(ref writer, "pickupedEntity", PickupedEntity);
        }

        public bool VerifyPrediction(ref CharacterPredictedState state)
        {
            return Position.Equals(state.Position) &&
                   Rotation.Equals(state.Rotation) &&
                   TriggeredEntity.Equals(state.TriggeredEntity) &&
                   PickupedEntity.Equals(state.PickupedEntity);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<CharacterPredictedState>();
        }
    }
}