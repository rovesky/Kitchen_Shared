using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemMoveToTableSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Item>().ForEach((Entity entity,
                ref ItemPredictedState itemState,
                ref TriggerPredictedState triggerState,
                ref VelocityPredictedState velocityState) =>
            {
                if (itemState.Owner != Entity.Null)
                    return;

                if (velocityState.MotionType != MotionType.Dynamic)
                    return;

                if (triggerState.TriggeredEntity == Entity.Null)
                    return;

                var triggeredEntity = triggerState.TriggeredEntity;
                if (!EntityManager.HasComponent<TriggerData>(triggeredEntity))
                    return;

                var triggerData = EntityManager.GetComponentData<TriggerData>(triggeredEntity);
                if ((triggerData.Type & (int) TriggerType.Table) == 0)
                    return;

                var slot = EntityManager.GetComponentData<SlotPredictedState>(triggeredEntity);
                if (slot.FilledInEntity != Entity.Null)
                    return;

                FSLog.Info("ItemMoveToTableSystem OnUpdate!");
             
                EntityManager.AddComponentData(entity, new AttachToTableRequest()
                {
                    ItemEntity = entity,
                    SlotPos = triggerData.SlotPos
                });
                EntityManager.AddComponentData(triggeredEntity, new TableFilledInItemRequest()
                {
                    ItemEntity = entity
                });
            });
        }
    }
 
}