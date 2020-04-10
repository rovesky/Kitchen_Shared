using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct SliceSetting : IComponentData
    {
      
    }

    public struct SlicePredictedState : IComponentData, IPredictedState<SlicePredictedState>
    {
        public bool IsSlicing;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            IsSlicing = reader.ReadBoolean();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteBoolean("IsSlicing",IsSlicing);
        }

        public bool VerifyPrediction(ref SlicePredictedState state)
        {
            return IsSlicing.Equals(state.IsSlicing);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<SlicePredictedState>();
        }
    }


   
}