using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

    public struct CatchFire : IComponentData
    {
        public byte TotalTick;
    }


    public struct CatchFirePredictedState : IComponentData, IPredictedState<CatchFirePredictedState>
    {
        public bool IsCatchFire;
        public byte CurTick;  

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            IsCatchFire = reader.ReadBoolean();
            CurTick = reader.ReadByte();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteBoolean("IsCatchFire",IsCatchFire);
            writer.WriteByte("CurTick",CurTick);
        }

        public bool VerifyPrediction(ref CatchFirePredictedState state)
        {
            return IsCatchFire.Equals(state.IsCatchFire)&&
                   CurTick.Equals(state.CurTick);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<CatchFirePredictedState>();
        }
    }
}