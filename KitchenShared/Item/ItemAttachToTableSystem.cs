using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemAttachToTableSystem : SystemBase
    {
        private CharacterDishOutSystem characterDishOutSystem;

        protected override void OnCreate()
        {
             characterDishOutSystem = World.GetExistingSystem<CharacterDishOutSystem>();
        }
        protected override void OnUpdate()
        {
            Entities.WithAll<Item>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in OwnerPredictedState itemState,
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

                    if (!EntityManager.HasComponent<Table>(triggeredEntity))
                        return;

                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggeredEntity);
                    if (slot.FilledIn == Entity.Null)
                    {
                        ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                            entity, triggeredEntity, Entity.Null);
                    }
                    else
                    {
                        if (!HasComponent<Plate>(slot.FilledIn))
                            return;

                        //食物不能装盘返回
                        if (!HasComponent<CanDishOut>(entity))
                            return;
                        characterDishOutSystem.DishOut(slot.FilledIn, 
                            entity, Entity.Null, quaternion.identity);
                    }
                }).Run();
        }
    }
 
}