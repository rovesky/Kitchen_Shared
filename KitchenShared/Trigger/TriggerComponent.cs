using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct TriggerSetting : IComponentData
    {
        public float Distance;
    }

    public struct TriggerPredictedState : IComponentData, IPredictedState<TriggerPredictedState>
    {
        public Entity TriggeredEntity;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref TriggeredEntity);
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "triggerEntity", TriggeredEntity);
        }

        public bool VerifyPrediction(ref TriggerPredictedState state)
        {
            return TriggeredEntity.Equals(state.TriggeredEntity);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<TriggerPredictedState>();
        }
    }
}