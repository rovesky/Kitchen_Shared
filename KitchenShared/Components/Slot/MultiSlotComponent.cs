using System;
using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{

    public struct MultiSlot
    {
        // 放入的对象
        public Entity FilledIn1;
        public Entity FilledIn2;
        public Entity FilledIn3;
        public Entity FilledIn4;


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

        public void Clear()
          {
              FilledIn1 = Entity.Null;
              FilledIn2 = Entity.Null;
              FilledIn3 = Entity.Null;
              FilledIn4 = Entity.Null;
          }

        private static bool IsSame(EntityManager entityManager,Entity entity, Entity entity1)
        {
            if (entity == Entity.Null)
                return false;

            var food = entityManager.GetComponentData<GameEntity>(entity);
            var food1 = entityManager.GetComponentData<GameEntity>(entity1);
            FSLog.Info($"food1.Type:{food1.Type},food.Type:{food.Type}");
            return food1.Type == food.Type;
        }
        
        public bool IsDuplicate(EntityManager entityManager, Entity entity)
        {
            return IsSame(entityManager,FilledIn1,entity)
                   || IsSame(entityManager,FilledIn2,entity)
                   || IsSame(entityManager,FilledIn3,entity)
                   || IsSame(entityManager,FilledIn4,entity);
        }

        
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

        public bool VerifyPrediction(ref MultiSlot state)
        {
            return FilledIn1.Equals(state.FilledIn1) &&
                   FilledIn2.Equals(state.FilledIn2) &&
                   FilledIn3.Equals(state.FilledIn3) &&
                   FilledIn4.Equals(state.FilledIn4);
        }
    }
    

    public struct MultiSlotPredictedState : IComponentData, IPredictedState<MultiSlotPredictedState>
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

        public bool VerifyPrediction(ref MultiSlotPredictedState state)
        {
            return Value.VerifyPrediction(ref state.Value);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<MultiSlotPredictedState>();
        }


    }
}