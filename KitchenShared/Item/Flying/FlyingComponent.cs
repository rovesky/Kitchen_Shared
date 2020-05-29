using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct Flying : IComponentData
    {

    }

    public struct FlyingPredictedState : IComponentData, IPredictedState<FlyingPredictedState>
    {
  
        public bool IsFlying;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
        
            IsFlying = reader.ReadBoolean();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteBoolean("IsFlying",IsFlying);
        }

        public bool VerifyPrediction(ref FlyingPredictedState state)
        {
            return IsFlying.Equals(state.IsFlying);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<FlyingPredictedState>();
        }
    }

}