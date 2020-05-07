using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct Burnt : IComponentData
    {
       
    }
   
    public struct BurntPredictedState : IComponentData, IPredictedState<BurntPredictedState>
    {
     
        public bool IsBurnt;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
       
            IsBurnt = reader.ReadBoolean();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
           
            writer.WriteBoolean("IsBurnt",IsBurnt);
        }

        public bool VerifyPrediction(ref BurntPredictedState state)
        {
            return  IsBurnt.Equals(state.IsBurnt);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<BurntPredictedState>();
        }
    }
}