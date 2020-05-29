using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public enum TriggerType : uint
    {
        None = 0,
        Character = 1 << 0,
        Item = 1 << 1,
        Table = 1 << 2,
        EveryThing = 0xFFFFFFFF

    }
    public struct TriggerSetting : IComponentData
    {
        public float Distance;
        //是否在静止的时候触发
        public bool IsMotionLess;

        public TriggerType BelongsToType;

        public uint TriggerWithType;
    }

    public struct AllowTrigger : IComponentData
    {

    }

    public struct TriggerPredictedState : IComponentData, IPredictedState<TriggerPredictedState>
    {
       
        public Entity TriggeredEntity;
        public Entity PreTriggeredEntity;
        public bool IsAllowTrigger;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref TriggeredEntity);
            context.RefSerializer.DeserializeReference(ref reader, ref PreTriggeredEntity);
            IsAllowTrigger = reader.ReadBoolean();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "triggerEntity", TriggeredEntity);
            context.RefSerializer.SerializeReference(ref writer, "PreTriggeredEntity", PreTriggeredEntity);

            writer.WriteBoolean("IsAllowTrigger",IsAllowTrigger);
        }

        public bool VerifyPrediction(ref TriggerPredictedState state)
        {
            return TriggeredEntity.Equals(state.TriggeredEntity)&&
                   PreTriggeredEntity.Equals(state.PreTriggeredEntity)&&
                  IsAllowTrigger.Equals(IsAllowTrigger);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<TriggerPredictedState>();
        }
    }
}