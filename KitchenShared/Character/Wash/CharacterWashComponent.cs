using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct WashSetting : IComponentData
    {
      
    }

    public struct WashPredictedState : IComponentData, IPredictedState<WashPredictedState>
    {
        public bool IsWashing;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            IsWashing = reader.ReadBoolean();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteBoolean("IsWashing",IsWashing);
        }

        public bool VerifyPrediction(ref WashPredictedState state)
        {
            return IsWashing.Equals(state.IsWashing);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<WashPredictedState>();
        }
    }
}