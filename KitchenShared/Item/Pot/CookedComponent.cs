using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct CookedSetting : IComponentData
    {
        public byte TotalCookTick;
        public byte TotalFireAlertTick;
       
    }


    public struct CookedPredictedState : IComponentData, IPredictedState<CookedPredictedState>
    {
        public byte CurCookTick;
        public byte CurFireAlertTick;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            CurCookTick = reader.ReadByte();
            CurFireAlertTick = reader.ReadByte();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteByte("CurCookTick",CurCookTick);
            writer.WriteByte("CurFireAlertTick",CurFireAlertTick);
        }

        public bool VerifyPrediction(ref CookedPredictedState state)
        {
            return CurCookTick.Equals(state.CurCookTick)
                && CurFireAlertTick.Equals(state.CurFireAlertTick);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<CookedPredictedState>();
        }
    }
}