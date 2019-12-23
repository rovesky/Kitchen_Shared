using FootStone.ECS;
using Unity.Entities;



namespace FootStone.Kitchen
{
    public struct ItemPredictedState : IComponentData, IPredictedState<ItemPredictedState>
    {
        public Entity Owner;
        public Entity TriggeredEntity;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref Owner);
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "owner", Owner);
        }

        public bool VerifyPrediction(ref ItemPredictedState state)
        {
            return Owner.Equals(state.Owner);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<ItemPredictedState>();
        }
    }
}