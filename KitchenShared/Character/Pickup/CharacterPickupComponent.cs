using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct PickupSetting : IComponentData
    {
        public int Foo;
    }

    public struct PickupPredictedState : IComponentData, IPredictedState<PickupPredictedState>
    {
        public Entity PickupedEntity;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref PickupedEntity);
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "pickupedEntity", PickupedEntity);
        }

        public bool VerifyPrediction(ref PickupPredictedState state)
        {
            return PickupedEntity.Equals(state.PickupedEntity);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<PickupPredictedState>();
        }
    }
}