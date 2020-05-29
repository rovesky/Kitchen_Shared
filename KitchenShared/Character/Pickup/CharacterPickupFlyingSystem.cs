using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupFlyingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            //开始接飞来的食物
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterPickupFlyingBegin")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref TriggerPredictedState triggerState,
                    ref CharacterMovePredictedState movePredictData,
                    ref TransformPredictedState transformState,
                    in CharacterMoveSetting moveSetting,
                    in SlotPredictedState slotState,
                    in UserCommand command) =>
                {
                    //  FSLog.Info($"CharacterPickupFlyingSystem,entity:{entity}");
                    //if (command.Buttons.IsSet(UserCommand.Button.Button2))
                    //   return;

                    var pickupEntity = slotState.FilledIn;

                    if (pickupEntity != Entity.Null || triggerState.TriggeredEntity == Entity.Null)
                        return;

                    if (!HasComponent<Flying>(triggerState.TriggeredEntity))
                        return;

                    if (!HasComponent<Item>(triggerState.TriggeredEntity))
                        return;

                    var item = EntityManager.GetComponentData<OwnerPredictedState>(triggerState.TriggeredEntity);
                    //  FSLog.Info($"PickUpItem flying,PreOwner:{item.PreOwner},entity:{entity}");
                    if (item.PreOwner == Entity.Null || item.PreOwner == entity)
                        return;

                    var worldTick = GetSingleton<WorldTime>().Tick;
                    FSLog.Info($"PickUpItem flying,command tick:{command.RenderTick},worldTick:{worldTick}");

                    ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                     triggerState.TriggeredEntity,entity,Entity.Null);
                    triggerState.TriggeredEntity = Entity.Null;
                    /*
                    var itemTransform = GetComponent<TransformPredictedState>(triggerState.TriggeredEntity);
                    var velocity = math.normalize(itemTransform.Position - transformState.Position) * 8.0f;
                    velocity.y = 0;
                    movePredictData.ImpulseVelocity = velocity;
                    movePredictData.ImpulseDuration = 5;

                    var fromRotation = transformState.Rotation;
                    var toRotation = quaternion.LookRotationSafe(velocity, math.up());
                    var angle = Quaternion.Angle(fromRotation, toRotation);
                    transformState.Rotation = Quaternion.RotateTowards(fromRotation, toRotation,
                        math.abs(angle - 180.0f) < float.Epsilon
                            ? -moveSetting.RotationVelocity
                            : moveSetting.RotationVelocity);*/

                

                }).Run();
        }
    }
}