using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupFlyingSystem : SystemBase
    {
        private EntityQuery flyingItemQuery;

        protected override void OnCreate()
        {
            flyingItemQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new[] {ComponentType.ReadOnly<Flying>()},
            });

            RequireForUpdate(flyingItemQuery);
        }

        protected override void OnUpdate()
        {
          /*  var flyingCount = flyingItemQuery.CalculateEntityCount();

            var flyingPos =
                new NativeArray<float3>(flyingCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);

            var initialFlyingPosJobHandle = Entities
                .WithName("InitialFlyingPosJobHandle")
                .ForEach((int entityInQueryIndex, in TransformPredictedState transformState) =>
                {
                    flyingPos[entityInQueryIndex] = transformState.Position;
                })
                .ScheduleParallel(Dependency);


            //开始接飞来的食物
            Dependency = Entities
                .WithAll<ServerEntity, Character>()
                .WithName("CharacterPickupFlyingBegin")
                .WithReadOnly(flyingPos)
                .ForEach((Entity entity,
                    ref TransformPredictedState transformState,
                    in CharacterMoveSetting moveSetting,
                    in SlotPredictedState slotState) =>
                {
                    var pickupEntity = slotState.FilledIn;

                    if (pickupEntity != Entity.Null)
                        return;

                    var closestPos = float3.zero;
                    var closestDistance = 5.0f;
                    for (var i = 0; i < flyingPos.Length; i++)
                    {
                        var itemPos = flyingPos[i];
                        var distance = math.distance(itemPos, transformState.Position);

                        if (distance >= closestDistance)
                            continue;

                        closestDistance = distance;
                        closestPos = itemPos;
                    }

                    if (closestPos.Equals(float3.zero))
                        return;

                    var dir = math.normalize(closestPos - transformState.Position);
                    dir.y = 0;
                    var fromRotation = transformState.Rotation;
                    var toRotation = quaternion.LookRotationSafe(dir, math.up());

                    var angle = Quaternion.Angle(fromRotation, toRotation);
                    transformState.Rotation = Quaternion.RotateTowards(fromRotation, toRotation,
                        math.abs(angle - 180.0f) < float.Epsilon
                            ? -moveSetting.RotationVelocity
                            : moveSetting.RotationVelocity);

                }).ScheduleParallel(initialFlyingPosJobHandle);

            CompleteDependency();*/

            //结束接飞来的食物
            Entities
                .WithAll<ServerEntity, Character>()
                .WithName("CharacterPickupFlyingEnd")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref TriggerPredictedState triggerState,
                    ref TransformPredictedState transformState,
                    in SlotPredictedState slotState) =>
                {

                    var pickupEntity = slotState.FilledIn;
                    if (pickupEntity != Entity.Null || triggerState.TriggeredEntity == Entity.Null)
                        return;

                    if (!HasComponent<Flying>(triggerState.TriggeredEntity))
                        return;

                    if (!HasComponent<Item>(triggerState.TriggeredEntity))
                        return;

                    var item = GetComponent<OwnerPredictedState>(triggerState.TriggeredEntity);
                    //  FSLog.Info($"PickUpItem flying,PreOwner:{item.PreOwner},entity:{entity}");
                    if (item.PreOwner == Entity.Null || item.PreOwner == entity)
                        return;
                    //  var worldTick = GetSingleton<WorldTime>().Tick;
                    //  FSLog.Info($"PickUpItem flying,command tick:{command.RenderTick},worldTick:{worldTick}");

                    var itemPos =  GetComponent<TransformPredictedState>(triggerState.TriggeredEntity);
                    var dir = math.normalize(itemPos.Position - transformState.Position);
                    dir.y = 0;

                    ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                        triggerState.TriggeredEntity, entity, Entity.Null);

                    transformState.Rotation = quaternion.LookRotationSafe(dir, math.up());
                    triggerState.TriggeredEntity = Entity.Null;

                }).Run();

           // flyingPos.Dispose();
        }
    }
}