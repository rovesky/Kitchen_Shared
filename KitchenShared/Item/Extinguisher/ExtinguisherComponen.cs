using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
   
    public struct ExtinguisherPredictedState : IComponentData, IPredictedState<ExtinguisherPredictedState>
    {
        public int Distance;
      
        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Distance = reader.ReadByte();
       
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteByte("Distance",(byte)Distance);
        }

        public bool VerifyPrediction(ref ExtinguisherPredictedState state)
        {
            return Distance.Equals(state.Distance);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<ExtinguisherPredictedState>();
        }
    }
   
}