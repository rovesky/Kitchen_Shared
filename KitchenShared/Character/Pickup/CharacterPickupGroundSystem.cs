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
                    //   in PickupSetting setting,
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
                    if (pickupedEntity == Entity.Null && triggerState.TriggeredEntity != Entity.Null)
                    {
                        if (!EntityManager.HasComponent<Item>(triggerState.TriggeredEntity))
                            return;
                        FSLog.Info(
                            $"PickUpItem,command ,triggerState.TriggeredEntity:{triggerState.TriggeredEntity},worldTick:{worldTick}");

                        ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                            triggerState.TriggeredEntity, entity, Entity.Null);


                        triggerState.TriggeredEntity = Entity.Null;
                    }
                    //putdown item
                    else if (pickupedEntity != Entity.Null && triggerState.TriggeredEntity == Entity.Null)
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