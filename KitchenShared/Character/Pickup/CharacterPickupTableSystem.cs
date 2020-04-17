﻿using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupTableSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterPickupTable")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in TriggerPredictedState triggerState,
                    in SlotPredictedState slotState,
                    in UserCommand command) =>
                {
                    //未按键返回
                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;

                    //没有触发返回
                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    //触发的不是Table返回
                    if (!EntityManager.HasComponent<Table>(triggerEntity))
                        return;

                    var worldTick = GetSingleton<WorldTime>().Tick;
                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);

                    var pickupEntity = slotState.FilledInEntity;
                    FSLog.Info($"worldTick:{worldTick},CharacterPickupTableSystem Update," +
                               $"PickupedEntity:{pickupEntity}," +
                               $"triggerEntity:{triggerEntity}，slot.FiltInEntity:{slot.FilledInEntity}");

                    if (pickupEntity == Entity.Null && slot.FilledInEntity != Entity.Null)
                    {
                        //the item is not sliced,can't pickup
                        if (EntityManager.HasComponent<FoodSlicedState>(slot.FilledInEntity))
                        {
                            var itemSliceState = EntityManager.GetComponentData<FoodSlicedState>(slot.FilledInEntity);
                            FSLog.Info($"PickUpItem,itemSliceState.CurSliceTick:{itemSliceState.CurSliceTick}");

                            if (itemSliceState.CurSliceTick > 0)
                                return;
                        }

                        FSLog.Info($"PickUpItem,command tick:{command.RenderTick},worldTick:{worldTick}");
                        ItemAttachUtilities.ItemAttachToOwner(EntityManager, 
                            slot.FilledInEntity, entity,triggerEntity);
                    }
                    else if (pickupEntity != Entity.Null && slot.FilledInEntity == Entity.Null)
                    {
                        ItemAttachUtilities.ItemAttachToOwner(EntityManager, 
                            pickupEntity, triggerEntity,entity);
                    }

                }).Run();
        }
      
    }
}