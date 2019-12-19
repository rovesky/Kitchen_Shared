using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct CharacterPredictedState : IComponentData, IPredictedState<CharacterPredictedState>
    {
        public Entity TriggeredEntity;
        public Entity PickupedEntity;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
        
            context.RefSerializer.DeserializeReference(ref reader, ref TriggeredEntity);
            context.RefSerializer.DeserializeReference(ref reader, ref PickupedEntity);
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "triggerEntity", TriggeredEntity);
            context.RefSerializer.SerializeReference(ref writer, "pickupedEntity", PickupedEntity);
        }

        public bool VerifyPrediction(ref CharacterPredictedState state)
        {
            return TriggeredEntity.Equals(state.TriggeredEntity) &&
                   PickupedEntity.Equals(state.PickupedEntity);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<CharacterPredictedState>();
        }
    }
}