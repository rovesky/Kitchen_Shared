using FootStone.ECS;
using Unity.Entities;



namespace FootStone.Kitchen
{
    public struct ItemPredictedState : IComponentData, IPredictedState<ItemPredictedState>
    {
        public Entity Owner;
        //public bool IsDynamic;
     
        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref Owner);
         //   IsDynamic = reader.ReadBoolean();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "owner", Owner);
           // writer.WriteBoolean("IsDynamic", IsDynamic);
        }

        public bool VerifyPrediction(ref ItemPredictedState state)
        {
            return Owner.Equals(state.Owner);// &&
            //    IsDynamic.Equals(state.IsDynamic);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<ItemPredictedState>();
        }
    }
}