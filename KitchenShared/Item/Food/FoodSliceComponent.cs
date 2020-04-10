using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{

    public struct Slice : IComponentData
    {
    
    }

    public struct Food : IComponentData
    {
    
    }

    public struct FoodSlicedSetting: IComponentData
    {
        public byte TotalSliceTick;
        public float3 OffPos;
    }

    public struct FoodSlicedRequest : IComponentData
    {
        public Entity Character;
    }

    public struct FoodSlicedState : IComponentData, IPredictedState<FoodSlicedState>
    {
        public byte CurSliceTick;
       
        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            CurSliceTick = reader.ReadByte();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
           writer.WriteByte("CurSliceTick",CurSliceTick);
        }

        public bool VerifyPrediction(ref FoodSlicedState state)
        {
            return CurSliceTick.Equals(state.CurSliceTick);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<FoodSlicedState>();
        }
    }
}