using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public enum ProgressType
    {
        None ,
        Slice ,
        Cook ,
        Wash
    }

    public struct ProgressSetting: IComponentData
    {
        public ProgressType Type;
        public byte TotalTick;
        public float3 OffPos;

    }
 

    public struct ProgressPredictState : IComponentData, IPredictedState<ProgressPredictState>
    {
        public byte CurTick;
       
        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            CurTick = reader.ReadByte();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
           writer.WriteByte("CurTick",CurTick);
        }

        public bool VerifyPrediction(ref ProgressPredictState state)
        {
            return CurTick.Equals(state.CurTick);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<ProgressPredictState>();
        }
    }
}