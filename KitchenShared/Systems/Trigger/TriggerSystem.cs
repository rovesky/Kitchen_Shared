using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class TriggerSystem : SystemBase
    {
        private KitchenBuildPhysicsWorld m_BuildPhysicsWorldSystem;
        private EntityQuery m_TriggerVolumeGroup;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<KitchenBuildPhysicsWorld>();
            m_TriggerVolumeGroup = GetEntityQuery(typeof(TriggerData));
        }

        protected override unsafe void OnUpdate()
        {
            var volumeEntities = m_TriggerVolumeGroup.ToEntityArray(Allocator.TempJob);
            var physicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld;

            Entities
                .WithAll<ServerEntity>()
                .WithReadOnly(volumeEntities)
                .ForEach((Entity entity,
                    ref TriggerPredictedState triggerState,
                    in TriggerSetting setting,
                    in TransformPredictedState transformState,
                    in PhysicsCollider collider) =>
                {
                    if (!triggerState.IsAllowTrigger)
                        return;

                    var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

                    // Character transform
                    var input = new ColliderDistanceInput
                    {
                        MaxDistance = setting.Distance,
                        Transform = new RigidTransform
                        {
                            pos = transformState.Position,
                            rot = transformState.Rotation
                        },
                        Collider = collider.ColliderPtr
                    };

                    var selfRigidBodyIndex = physicsWorld.GetRigidBodyIndex(entity);
                    physicsWorld.CalculateDistance(input, ref distanceHits);

                    // if (volumeEntities.Length > 0)
                    //FSLog.Info($"volumeEntities.Length:{volumeEntities.Length}");

                    var triggerIndex = CheckTrigger(ref physicsWorld, ref volumeEntities,
                        selfRigidBodyIndex, ref distanceHits);

                    triggerState.TriggeredEntity = triggerIndex < 0
                        ? Entity.Null
                        : physicsWorld.Bodies[distanceHits[triggerIndex].RigidBodyIndex].Entity;

                    // if (predictedState.TriggeredEntity != Entity.Null)
                    //FSLog.Info($"triggerEntity:{triggerState.TriggeredEntity}");

                    distanceHits.Dispose();
                })
                //   .WithDeallocateOnJobCompletion(volumeEntities)
                .Run();
            volumeEntities.Dispose();
        }

        private static int CheckTrigger(ref PhysicsWorld world, ref NativeArray<Entity> volumeEntities,
            int selfRigidBodyIndex, ref NativeList<DistanceHit> distanceHits)
        {
            var triggerIndex = -1;
            for (var i = 0; i < distanceHits.Length; i++)
            {
                var hit = distanceHits[i];
                if (hit.RigidBodyIndex == selfRigidBodyIndex)
                    continue;

                var e = world.Bodies[hit.RigidBodyIndex].Entity;

                if (!volumeEntities.Contains(e))
                    continue;

                if (triggerIndex < 0)
                    triggerIndex = i;
                else if (distanceHits[triggerIndex].Distance > hit.Distance)
                    triggerIndex = i;
            }

            return triggerIndex;
        }
    }
}