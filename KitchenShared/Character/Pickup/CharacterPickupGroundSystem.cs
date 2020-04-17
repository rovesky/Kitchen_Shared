using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupGroundSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,

                    ref TriggerPredictedState triggerState,
                    in SlotPredictedState slotState,
                    in UserCommand command,
                    in TransformPredictedState transformState,
                    in VelocityPredictedState velocityState) =>
                {
                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;

                    var worldTick = GetSingleton<WorldTime>().Tick;
                    //   FSLog.Info($"CharacterPickupGroundSystem:{pickupState.PickupedEntity},{triggerState.TriggeredEntity}");

                    //pickup item
                    var pickupedEntity = slotState.FilledInEntity;
                    var triggeredEntity = triggerState.TriggeredEntity;
                    if (pickupedEntity == Entity.Null && triggeredEntity != Entity.Null)
                    {
                        if (!EntityManager.HasComponent<Item>(triggeredEntity))
                            return;
                        FSLog.Info($"PickUpItem,command ,triggerState.TriggeredEntity:{triggeredEntity},worldTick:{worldTick}");

                        ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                            triggeredEntity, entity, Entity.Null);

                        triggerState.TriggeredEntity = Entity.Null;
                    }
                    //putdown item
                    else if (pickupedEntity != Entity.Null && triggeredEntity == Entity.Null)
                    {
                        FSLog.Info(
                            $"PutDownItem,tick:{command.RenderTick},worldTick:{worldTick},velocityState.Linear:{velocityState.Linear}");

                        ItemAttachUtilities.ItemDetachFromOwner(EntityManager,
                            pickupedEntity,
                            entity,
                            transformState.Position + math.mul(transformState.Rotation, new float3(0, -0.2f, 1.3f)),
                            velocityState.Linear);

                    }
                }).Run();
        }
    }
}