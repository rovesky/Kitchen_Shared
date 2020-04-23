using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct SlotSetting : IComponentData
    {
        public float3 Pos;
        public float3 Offset;
    }

    public struct SlotPredictedState : IComponentData, IPredictedState<SlotPredictedState>
    {
        // 放入的对象
        public Entity FilledIn;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref FilledIn);
           // FSLog.Info($"SlotPredictedState DeserializeReference:{FilledInEntity}");
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "FilledIn", FilledIn);
        }

        public bool VerifyPrediction(ref SlotPredictedState state)
        {
            return FilledIn.Equals(state.FilledIn);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<SlotPredictedState>();
        }


    }
}