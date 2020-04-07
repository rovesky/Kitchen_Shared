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
                if (!EntityManager.HasComponent<TriggeredSetting>(triggeredEntity))
                    return;

              //  var triggerData = EntityManager.GetComponentData<TriggeredSetting>(triggeredEntity);
               // if ((triggerData.Type & (int) TriggerType.Table) == 0)
                //    return;

                if(!EntityManager.HasComponent<Table>(triggeredEntity))
                    return;

                var slot = EntityManager.GetComponentData<SlotPredictedState>(triggeredEntity);
                if (slot.FilledInEntity != Entity.Null)
                    return;

                FSLog.Info("ItemMoveToTableSystem OnUpdate!");
                var slotSetting =  EntityManager.GetComponentData<SlotSetting>(triggeredEntity);

                ItemAttachUtilities.ItemAttachToTable(EntityManager, entity, slotSetting.Pos);

                slot.FilledInEntity = entity;
                EntityManager.SetComponentData(triggeredEntity,slot);
            
            }).Run();
        }
    }
 
}