using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct ItemPredictedState : IComponentData, IPredictedState<ItemPredictedState>
    {
        public Entity Owner;
        public Entity PreOwner;
       
        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref Owner);
            context.RefSerializer.DeserializeReference(ref reader, ref PreOwner);
            
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "Owner", Owner);
            context.RefSerializer.SerializeReference(ref writer, "PreOwner", PreOwner);
        }

        public bool VerifyPrediction(ref ItemPredictedState state)
        {
            return Owner.Equals(state.Owner) &&
                   PreOwner.Equals(state.PreOwner);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<ItemPredictedState>();
        }
    }
}