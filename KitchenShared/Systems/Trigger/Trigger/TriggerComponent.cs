using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct TriggerSetting : IComponentData
    {
        public float Distance;
    }

    public struct AllowTrigger : IComponentData
    {

    }

    public struct TriggerPredictedState : IComponentData, IPredictedState<TriggerPredictedState>
    {
        public Entity TriggeredEntity;
        public bool IsAllowTrigger;
        public float3 LastPos;

       // public Entity PreTriggeredEntity;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref TriggeredEntity);
            IsAllowTrigger = reader.ReadBoolean();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "triggerEntity", TriggeredEntity);
            writer.WriteBoolean("IsAllowTrigger",IsAllowTrigger);
        }

        public bool VerifyPrediction(ref TriggerPredictedState state)
        {
            return TriggeredEntity.Equals(state.TriggeredEntity)&&
                  IsAllowTrigger.Equals(IsAllowTrigger);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<TriggerPredictedState>();
        }
    }
}