using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

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
                    int entityInQueryIndex,
                    ref PickupPredictedState pickupState,
                    in PickupSetting setting,
                    in TriggerPredictedState triggerState,
                    in ReplicatedEntityData replicatedEntityData,
                    in UserCommand command) =>
                {

                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;

                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                   // var triggerData = EntityManager.GetComponentData<TriggeredSetting>(triggerEntity);
                   // if ((triggerData.Type & (int) TriggerType.Table) == 0)
                  //      return;
                    if(!EntityManager.HasComponent<Table>(triggerEntity))
                        return;

                    var worldTick = GetSingleton<WorldTime>().Tick;
                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);

                    FSLog.Info($"worldTick:{worldTick},TriggerOperationSystem Update," +
                        $"PickupedEntity:{pickupState.PickupedEntity}," +
                        $"triggerEntity:{triggerEntity}，slot.FiltInEntity:{slot.FilledInEntity}");

                    if (pickupState.PickupedEntity == Entity.Null && slot.FilledInEntity != Entity.Null)
                    {
                      
                        //the item is not sliced,can't pickup
                        if (EntityManager.HasComponent<FoodSliceState>(slot.FilledInEntity))
                        {
                            var itemSliceState = EntityManager.GetComponentData<FoodSliceState>(slot.FilledInEntity);
                            FSLog.Info($"PickUpItem,itemSliceState.CurSliceTick:{itemSliceState.CurSliceTick}");

                            if (itemSliceState.CurSliceTick > 0)
                                return;
                        }

                        FSLog.Info($"PickUpItem,command tick:{command.RenderTick},worldTick:{worldTick}");
                        
                        ItemAttachUtilities.ItemAttachToCharacter(EntityManager, slot.FilledInEntity, entity,
                            replicatedEntityData.PredictingPlayerId);

                        pickupState.PickupedEntity = slot.FilledInEntity;

                        slot.FilledInEntity = Entity.Null;
                        EntityManager.SetComponentData(triggerEntity, slot);
                    }
                    else if (pickupState.PickupedEntity != Entity.Null && slot.FilledInEntity == Entity.Null)
                    {
                        FSLog.Info($"PutDownItem,tick:{command.RenderTick},worldTick:{worldTick}");

                        ItemAttachUtilities.ItemDetachFromCharacter(EntityManager, pickupState.PickupedEntity,
                            Entity.Null, float3.zero, float3.zero);

                     
                        var slotSetting =  EntityManager.GetComponentData<SlotSetting>(triggerEntity);
                        ItemAttachUtilities.ItemAttachToTable(EntityManager, pickupState.PickupedEntity,slotSetting.Pos);

                        slot.FilledInEntity = pickupState.PickupedEntity;
                        EntityManager.SetComponentData(triggerEntity, slot);

                        pickupState.PickupedEntity = Entity.Null;
                    }

                }).Run();
        }
    }
}