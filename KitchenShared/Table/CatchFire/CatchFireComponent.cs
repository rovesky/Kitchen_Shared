using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

    public struct CatchFire : IComponentData
    {
       
    }


    public struct CatchFireSetting: IComponentData
    {
        public ushort  TotalExtinguishTick;
        public ushort  FireSpreadTick;
        public float FireSpreadRadius;
    }

    public struct CatchFirePredictedState : IComponentData, IPredictedState<CatchFirePredictedState>
    {
        public bool IsCatchFire;
        public ushort CurCatchFireTick;  
        public ushort CurExtinguishTick;  

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            IsCatchFire = reader.ReadBoolean();
            CurCatchFireTick = reader.ReadUInt16();
            CurExtinguishTick = reader.ReadUInt16();
        
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteBoolean("IsCatchFire",IsCatchFire);
            writer.WriteUInt16("CurCatchFireTick", CurCatchFireTick);
            writer.WriteUInt16("CurExtinguishTick",CurExtinguishTick);
        }

        public bool VerifyPrediction(ref CatchFirePredictedState state)
        {
            return IsCatchFire.Equals(state.IsCatchFire)&&
                   CurCatchFireTick.Equals(state.CurCatchFireTick)&&
                   CurExtinguishTick.Equals(state.CurExtinguishTick);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<CatchFirePredictedState>();
        }
    }
}