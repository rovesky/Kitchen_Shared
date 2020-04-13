using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterServeSystem : SystemBase 
    {
        private void PutDownItem(ref PickupPredictedState pickupState,ref SlotPredictedState slot,Entity triggerEntity)
        {
            FSLog.Info($"PutDownItem ,triggerEntity:{triggerEntity}");

            ItemAttachUtilities.ItemDetachFromCharacter(EntityManager, pickupState.PickupedEntity,
                Entity.Null, float3.zero, float3.zero);
                        

            var slotSetting = EntityManager.GetComponentData<SlotSetting>(triggerEntity);
            ItemAttachUtilities.ItemAttachToTable(EntityManager, pickupState.PickupedEntity,
                triggerEntity,slotSetting.Pos);

            slot.FilledInEntity = pickupState.PickupedEntity;
            EntityManager.SetComponentData(triggerEntity, slot);

            pickupState.PickupedEntity = Entity.Null;
        }

        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterServeSystem")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    int entityInQueryIndex,
                    ref PickupPredictedState pickupState,
                    in ServeSetting setting,
                    in TriggerPredictedState triggerState,
                    in ReplicatedEntityData replicatedEntityData,
                    in UserCommand command) =>
                {

                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;

                    if (pickupState.PickupedEntity == Entity.Null)
                        return;
                   // FSLog.Info("CharacterServeSystem 1");
                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                   // FSLog.Info("CharacterServeSystem 2");
                    if(!EntityManager.HasComponent<TableServe>(triggerEntity))
                        return;

                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);
                    if(slot.FilledInEntity != Entity.Null)
                        return;

                  //  FSLog.Info("CharacterServeSystem 3");

                    if(!EntityManager.HasComponent<Plate>(pickupState.PickupedEntity))
                        return;
                  //  FSLog.Info("CharacterServeSystem 4");
                    PutDownItem(ref pickupState,ref slot, triggerEntity);

                }).Run();
        }

    }
}