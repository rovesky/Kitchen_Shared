using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct Burnt : IComponentData
    {
       
    }

    public enum PotState
    {
        Empty,
        Full,
        Cooked,
        Burnt
    }
   
    public struct PotPredictedState : IComponentData, IPredictedState<PotPredictedState>
    {
     
        public PotState State;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
       
            State = (PotState)reader.ReadByte();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
           
            writer.WriteByte("PotState",(byte)State);
        }

        public bool VerifyPrediction(ref PotPredictedState state)
        {
            return  State.Equals(state.State);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<PotPredictedState>();
        }
    }
}