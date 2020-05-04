using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct FireAlertSetting : IComponentData
    {
        public byte TotalTick;
    }

    public struct FireAlertPredictedState : IComponentData, IPredictedState<FireAlertPredictedState>
    {
     
        public byte CurTick;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
       
            CurTick = reader.ReadByte();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
           
            writer.WriteByte("CurFireAlertTick",CurTick);
        }

        public bool VerifyPrediction(ref FireAlertPredictedState state)
        {
            return  CurTick.Equals(state.CurTick);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<FireAlertPredictedState>();
        }
    }
}