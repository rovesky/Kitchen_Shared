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
                    //   ref CharacterMovePredictedState movePredictData,
                    ref TransformPredictedState transformState,
                    //   in CharacterMoveSetting moveSetting,
                    in SlotPredictedState slotState,
                    in UserCommand command) =>
                {
                    //  FSLog.Info($"CharacterPickupFlyingSystem,entity:{entity}");
                    //if (command.Buttons.IsSet(UserCommand.Button.Button2))
                    //   return;
                    var pickupEntity = slotState.FilledIn;

                    if (pickupEntity != Entity.Null || triggerState.TriggeredEntity == Entity.Null)
                        return;
                    //   FSLog.Info($"PickUpItem flying1,command tick:{command.RenderTick}");

                    if (!HasComponent<Flying>(triggerState.TriggeredEntity))
                        return;
                    // FSLog.Info($"PickUpItem flying2,command tick:{command.RenderTick}");

                    if (!HasComponent<Item>(triggerState.TriggeredEntity))
                        return;

                    var item = EntityManager.GetComponentData<OwnerPredictedState>(triggerState.TriggeredEntity);
                    FSLog.Info($"PickUpItem flying,PreOwner:{item.PreOwner},entity:{entity}");
                    if (item.PreOwner == Entity.Null || item.PreOwner == entity)
                        return;

                    var worldTick = GetSingleton<WorldTime>().Tick;
                    FSLog.Info($"PickUpItem flying,command tick:{command.RenderTick},worldTick:{worldTick}");

                    var itemTransform = GetComponent<TransformPredictedState>(triggerState.TriggeredEntity);
                    var velocity = math.normalize(itemTransform.Position - transformState.Position);
                    velocity.y = 0;
              
                    //var angle = Quaternion.Angle(fromRotation, toRotation);
                    //transformState.Rotation = Quaternion.RotateTowards(fromRotation, toRotation,
                    //    math.abs(angle - 180.0f) < float.Epsilon
                    //        ? -moveSetting.RotationVelocity
                    //        : moveSetting.RotationVelocity);
                    ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                        triggerState.TriggeredEntity, entity, Entity.Null);

                    var fromRotation = transformState.Rotation;
                    var toRotation = quaternion.LookRotationSafe(velocity, math.up());
                    transformState.Rotation = toRotation;

                    triggerState.TriggeredEntity = Entity.Null;

                }).Run();
        }
    }
}