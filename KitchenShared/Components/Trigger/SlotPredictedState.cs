using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct SlotPredictedState : IComponentData, /*IReplicatedState*/IPredictedState<SlotPredictedState>
    {
        // ���
       // public float3 SlotPos;

        // ����Ķ���
        public Entity FilledInEntity;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref FilledInEntity);
            FSLog.Info($"SlotPredictedState DeserializeReference:{FilledInEntity}");
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "filledInEntity", FilledInEntity);
        }

        public bool VerifyPrediction(ref SlotPredictedState state)
        {
            return FilledInEntity.Equals(state.FilledInEntity);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<SlotPredictedState>();
        }

        //public static IReplicatedStateSerializerFactory CreateSerializerFactory()
        //{
        //    return new ReplicatedStateSerializerFactory<SlotPredictedState>();
        //}
    }
}