﻿using FootStone.ECS;
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

                    var triggerData = EntityManager.GetComponentData<TriggerData>(triggerEntity);
                    if ((triggerData.Type & (int) TriggerType.Table) == 0)
                        return;

                    var worldTick = GetSingleton<WorldTime>().Tick;
                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);

                    FSLog.Info(
                        $"worldTick:{worldTick},TriggerOperationSystem Update,PickupedEntity:{pickupState.PickupedEntity}," +
                        $"triggerEntity:{triggerEntity}，slot.FiltInEntity:{slot.FilledInEntity}");

                    if (pickupState.PickupedEntity == Entity.Null && slot.FilledInEntity != Entity.Null)
                    {

                        FSLog.Info($"PickUpItem,command tick:{command.RenderTick},worldTick:{worldTick}");
                        //if(EntityManager.HasComponent<TableFilledInItemRequest>(triggerEntity) )
                        EntityManager.AddComponentData(triggerEntity, new TableFilledInItemRequest
                        {
                            ItemEntity = Entity.Null
                        });

                        EntityManager.AddComponentData(slot.FilledInEntity, new ItemAttachToCharacterRequest
                        {
                            PredictingPlayerId = replicatedEntityData.PredictingPlayerId,
                            Owner = entity
                        });

                        pickupState.PickupedEntity = slot.FilledInEntity;
                    }
                    else if (pickupState.PickupedEntity != Entity.Null && slot.FilledInEntity == Entity.Null)
                    {
                        FSLog.Info($"PutDownItem,tick:{command.RenderTick},worldTick:{worldTick}");

                        EntityManager.AddComponentData(pickupState.PickupedEntity, new ItemDetachFromCharacterRequest
                        {
                            Pos = float3.zero,
                            LinearVelocity = float3.zero
                        });

                        EntityManager.AddComponentData(triggerEntity, new TableFilledInItemRequest
                        {
                            ItemEntity = pickupState.PickupedEntity
                        });

                        EntityManager.AddComponentData(pickupState.PickupedEntity, new ItemAttachToTableRequest
                        {
                        //    ItemEntity = pickupState.PickupedEntity,
                            SlotPos = triggerData.SlotPos
                        });

                        pickupState.PickupedEntity = Entity.Null;
                    }
                }).Run();
        }

    }
}