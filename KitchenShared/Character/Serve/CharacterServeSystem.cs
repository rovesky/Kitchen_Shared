using FootStone.ECS;
using Unity.Entities;


namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterServeSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterServeSystem")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in SlotPredictedState slotState,
                    in ServeSetting setting,
                    in TriggerPredictedState triggerState,
                    in UserCommand command) =>
                {

                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;

                    var pickupedEntity = slotState.FilledInEntity;
                    if (pickupedEntity == Entity.Null)
                        return;

                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    if (!EntityManager.HasComponent<TableServe>(triggerEntity))
                        return;

                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);
                    if (slot.FilledInEntity != Entity.Null)
                        return;

                    if (!EntityManager.HasComponent<Plate>(pickupedEntity))
                        return;

                    ItemAttachUtilities.ItemAttachToOwner(EntityManager, pickupedEntity,
                        triggerEntity, entity);

                }).Run();
        }

    }
}