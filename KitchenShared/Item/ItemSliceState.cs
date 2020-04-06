using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct ItemSliceSetting: IComponentData
    {
        public byte TotalSliceTick;
        public float3 OffPos;
    }


    public struct ItemSliceState : IComponentData, IPredictedState<ItemSliceState>
    {
        public byte CurSliceTick;
        public bool IsSlicing;
       
        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {

            CurSliceTick = reader.ReadByte();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
           writer.WriteByte("Percentage",CurSliceTick);
        }

        public bool VerifyPrediction(ref ItemSliceState state)
        {
            return CurSliceTick.Equals(state.CurSliceTick);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<ItemSliceState>();
        }
    }
}