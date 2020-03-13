using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemMoveToTableSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<Item>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in ItemPredictedState itemState,
                    in TriggerPredictedState triggerState,
                    in VelocityPredictedState velocityState) =>
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
             
                EntityManager.AddComponentData(entity, new ItemAttachToTableRequest()
                {
                 //   ItemEntity = entity,
                    SlotPos = triggerData.SlotPos
                });
                EntityManager.AddComponentData(triggeredEntity, new TableFilledInItemRequest()
                {
                    ItemEntity = entity
                });
            }).Run();
        }
    }
 
}