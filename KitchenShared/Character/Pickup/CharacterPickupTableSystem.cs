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

                    var triggerData = EntityManager.GetComponentData<TriggeredSetting>(triggerEntity);
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

                        ItemUtilities.ItemAttachToCharacter(EntityManager, slot.FilledInEntity, entity,
                            replicatedEntityData.PredictingPlayerId);

                        pickupState.PickupedEntity = slot.FilledInEntity;

                        slot.FilledInEntity = Entity.Null;
                        EntityManager.SetComponentData(triggerEntity, slot);
                    }
                    else if (pickupState.PickupedEntity != Entity.Null && slot.FilledInEntity == Entity.Null)
                    {
                        FSLog.Info($"PutDownItem,tick:{command.RenderTick},worldTick:{worldTick}");

                        ItemUtilities.ItemDetachFromCharacter(EntityManager, pickupState.PickupedEntity,
                            Entity.Null, float3.zero, float3.zero);

                        ItemUtilities.ItemAttachToTable(EntityManager, pickupState.PickupedEntity, triggerData.SlotPos);

                        slot.FilledInEntity = pickupState.PickupedEntity;
                        EntityManager.SetComponentData(triggerEntity, slot);

                        pickupState.PickupedEntity = Entity.Null;
                    }

                }).Run();
        }
    }
}