using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
  
    public struct SinkSetting : IComponentData
    {
        public float3 SlotWashed;
        public float3 SlotWashing;

    }


    public struct SinkPredictedState : IComponentData, IPredictedState<SinkPredictedState>
    {
        public MultiSlot Value;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Value.Deserialize(ref context, ref reader);
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            Value.Serialize(ref context,ref writer);

        }

        public bool VerifyPrediction(ref SinkPredictedState state)
        {
            return Value.VerifyPrediction(ref state.Value);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<SinkPredictedState>();
        }


    }
}