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


    public struct MultiSlotSetting : IComponentData
    {
        public float3 Pos;
        public float3 Offset;
    }

    public struct MultiSlotPredictedState : IComponentData, IPredictedState<MultiSlotPredictedState>
    {
        // 放入的对象
        public Entity FilledIn1;
        public Entity FilledIn2;
        public Entity FilledIn3;
        public Entity FilledIn4;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref FilledIn1);
            context.RefSerializer.DeserializeReference(ref reader, ref FilledIn2);
            context.RefSerializer.DeserializeReference(ref reader, ref FilledIn3);
            context.RefSerializer.DeserializeReference(ref reader, ref FilledIn4);

            // FSLog.Info($"SlotPredictedState DeserializeReference:{FilledInEntity}");
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "FilledIn1", FilledIn1);
            context.RefSerializer.SerializeReference(ref writer, "FilledIn2", FilledIn2);
            context.RefSerializer.SerializeReference(ref writer, "FilledIn3", FilledIn3);
            context.RefSerializer.SerializeReference(ref writer, "FilledIn4", FilledIn4);
        }

        public bool VerifyPrediction(ref MultiSlotPredictedState state)
        {
            return FilledIn1.Equals(state.FilledIn1) &&
                   FilledIn2.Equals(state.FilledIn2) &&
                   FilledIn3.Equals(state.FilledIn3) &&
                   FilledIn4.Equals(state.FilledIn4);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<MultiSlotPredictedState>();
        }

        public int Count()
        {
            var count = 0;

            if (FilledIn1 != Entity.Null)
                count++;
            if (FilledIn2 != Entity.Null)
                count++;
            if (FilledIn3 != Entity.Null)
                count++;
            if (FilledIn4 != Entity.Null)
                count++;

            return count;
        }

        public bool IsFull()
        {
            return FilledIn1 != Entity.Null &&
                   FilledIn2 != Entity.Null &&
                   FilledIn3 != Entity.Null &&
                   FilledIn4 != Entity.Null;
        }

        public bool IsEmpty()
        {
            return FilledIn1 == Entity.Null &&
                   FilledIn2 == Entity.Null &&
                   FilledIn3 == Entity.Null &&
                   FilledIn4 == Entity.Null;
        }

        public void FillIn(Entity entity)
        {
            if(IsFull())
                return;

            if (FilledIn1 == Entity.Null)
            {
                FilledIn1 = entity;
                return;
            }

            if (FilledIn2 == Entity.Null)
            {
                FilledIn2 = entity;
                return;
            }

            if (FilledIn3 == Entity.Null)
            {
                FilledIn3 = entity;
                return;
            }

            if (FilledIn4 == Entity.Null)
            {
                FilledIn4 = entity;
            }
        }

        public Entity TakeOut()
        {
            if(IsEmpty())
                return Entity.Null;

            var ret = Entity.Null;
            if (FilledIn4 != Entity.Null)
            {
                ret = FilledIn4;
                FilledIn4 = Entity.Null;
                return ret;
            }

            if (FilledIn3 != Entity.Null)
            {
                ret = FilledIn3;
                FilledIn3 = Entity.Null;
                return ret;
            }

            if (FilledIn2 != Entity.Null)
            {
                ret = FilledIn2;
                FilledIn2 = Entity.Null;
                return ret;
            }

            if (FilledIn1 != Entity.Null)
            {
                ret = FilledIn1;
                FilledIn1 = Entity.Null;
                return ret;
            }

            return ret;
        }

        public Entity GetTail()
        {
            if (FilledIn4 != Entity.Null)
                return FilledIn4;

            if (FilledIn3 != Entity.Null)
                return FilledIn3;

            if (FilledIn2 != Entity.Null)
                return FilledIn2;

            if (FilledIn1 != Entity.Null)
                return FilledIn1;

            return Entity.Null;
        }
    }
}