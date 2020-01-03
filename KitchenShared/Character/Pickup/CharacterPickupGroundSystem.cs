using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupGroundSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<ServerEntity>().ForEach((Entity entity,
                ref PickupSetting setting,
                ref UserCommand command,
                ref PickupPredictedState pickupState,
                ref TriggerPredictedState triggerState,
                ref TransformPredictedState entityPredictData) =>
            {
                if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                    return;

                var worldTick = GetSingleton<WorldTime>().Tick;
             //   FSLog.Info($"CharacterPickupGroundSystem:{pickupState.PickupedEntity},{triggerState.TriggeredEntity}");
                if (pickupState.PickupedEntity == Entity.Null
                    && triggerState.TriggeredEntity != Entity.Null)
                {
                    var triggerData = EntityManager.GetComponentData<TriggerData>(triggerState.TriggeredEntity);
                    if ((triggerData.Type & (int) TriggerType.Item) == 0)
                        return;

                    FSLog.Info($"PickUpItem,command tick:{command.RenderTick},worldTick:{worldTick}");
                    // PickUpItem(entity,ref triggerState, ref pickupState);
                    var ownerReplicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(entity);
                    EntityManager.AddComponentData(triggerState.TriggeredEntity, new AttachToCharacterRequest
                    {
                        PredictingPlayerId = ownerReplicatedEntityData.PredictingPlayerId,
                        Owner = entity
                    });

                    pickupState.PickupedEntity = triggerState.TriggeredEntity;
                }
                else if (pickupState.PickupedEntity != Entity.Null
                         && triggerState.TriggeredEntity == Entity.Null)
                {
                    FSLog.Info($"PutDownItem,tick:{command.RenderTick},worldTick:{worldTick}");

                    EntityManager.AddComponentData(pickupState.PickupedEntity, new DetachFromCharacterRequest()
                    {
                        Pos = entityPredictData.Position +
                              math.mul(entityPredictData.Rotation, new float3(0, -0.2f, 1.3f)),
                        LinearVelocity = float3.zero
                            
                    });

                    pickupState.PickupedEntity = Entity.Null;
                }
            });
        }
    }
}