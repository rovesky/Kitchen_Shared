using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct ItemPredictedState : IComponentData, IPredictedState<ItemPredictedState>
    {
        public Entity Owner;
        public Entity TempOwner;
       
        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref Owner);
            context.RefSerializer.DeserializeReference(ref reader, ref TempOwner);
            
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "owner", Owner);
            context.RefSerializer.SerializeReference(ref writer, "TempOwner", TempOwner);
        }

        public bool VerifyPrediction(ref ItemPredictedState state)
        {
            return Owner.Equals(state.Owner) &&
                   TempOwner.Equals(state.TempOwner);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<ItemPredictedState>();
        }
    }
}