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
                ref PickupPredictedState pickupState,
                ref TriggerPredictedState triggerState,
                in PickupSetting setting,
                in UserCommand command,
                in TransformPredictedState transformState,
                in VelocityPredictedState velocityState,
                in ReplicatedEntityData replicatedEntityData) =>
            {
                if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                    return;

                var worldTick = GetSingleton<WorldTime>().Tick;
                //   FSLog.Info($"CharacterPickupGroundSystem:{pickupState.PickupedEntity},{triggerState.TriggeredEntity}");

                //pickup item
                if (pickupState.PickupedEntity == Entity.Null
                    && triggerState.TriggeredEntity != Entity.Null)
                {
                    //var triggerData = EntityManager.GetComponentData<TriggeredSetting>(triggerState.TriggeredEntity);
                    //if ((triggerData.Type & (int) TriggerType.Item) == 0)
                    //    return;
                    if(!EntityManager.HasComponent<Item>(triggerState.TriggeredEntity))
                        return;
                    FSLog.Info($"PickUpItem,command ,triggerState.TriggeredEntity:{triggerState.TriggeredEntity},worldTick:{worldTick}");

                    ItemAttachUtilities.ItemAttachToCharacter(EntityManager, triggerState.TriggeredEntity, entity,
                        replicatedEntityData.PredictingPlayerId);

                    pickupState.PickupedEntity = triggerState.TriggeredEntity;
                    triggerState.TriggeredEntity = Entity.Null;
                }
                //putdown item
                else if (pickupState.PickupedEntity != Entity.Null
                         && triggerState.TriggeredEntity == Entity.Null)
                {
                    FSLog.Info(
                        $"PutDownItem,tick:{command.RenderTick},worldTick:{worldTick},velocityState.Linear:{velocityState.Linear}");
                    
                    ItemAttachUtilities.ItemDetachFromCharacter(EntityManager, 
                        pickupState.PickupedEntity, 
                        Entity.Null,
                        transformState.Position + math.mul(transformState.Rotation, new float3(0, -0.2f, 1.3f)),
                        velocityState.Linear);

                    pickupState.PickupedEntity = Entity.Null;
                }
            }).Run();
        }
    }
}