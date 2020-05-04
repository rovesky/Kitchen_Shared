using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Transforms;
using UnityEngine;
using RaycastHit = Unity.Physics.RaycastHit;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class TriggerEntitiesSystem : SystemBase
    {
        private KitchenBuildPhysicsWorld m_BuildPhysicsWorldSystem;
        private EntityQuery m_TriggerVolumeGroup;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<KitchenBuildPhysicsWorld>();
            m_TriggerVolumeGroup = GetEntityQuery(typeof(TriggeredSetting));
        }

        protected override unsafe void OnUpdate()
        {
            var volumeEntities = m_TriggerVolumeGroup.ToEntityArray(Allocator.TempJob);
            var physicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld;

            Entities
                .WithAll<ServerEntity>()
                .WithStructuralChanges()
                .WithReadOnly(volumeEntities)
                .ForEach((Entity entity,
                    in ExtinguisherPredictedState extinguisherState,
                  //  in TransformPredictedState transformState,
                    in LocalToWorld localToWorld,
                    in PhysicsCollider collider) =>
                {
                    if (extinguisherState.Distance == 0)
                        return;

                    var distanceHits = new NativeList<RaycastHit>(Allocator.Temp);

                    var pos = localToWorld.Position;
                    pos.y = 0.1f;
                    // Character transform
                    var input = new RaycastInput
                  
                        {
                            Start = pos,
                            End = pos + math.forward(localToWorld.Rotation) * extinguisherState.Distance,
                            Filter = CollisionFilter.Default
                        };

               

                    var selfRigidBodyIndex = physicsWorld.GetRigidBodyIndex(entity);

                    if (!physicsWorld.CollisionWorld.CastRay(input, ref distanceHits))
                    {
                        FSLog.Info($"TriggerEntitiesSystem fail");

                        return;
                    }


                    FSLog.Info($"TriggerEntitiesSystem,Distance:{extinguisherState.Distance},distanceHits.Length:{distanceHits.Length}");
                    for (var i = 0; i < distanceHits.Length; i++)
                    {
                        var hit = distanceHits[i];
                        if (hit.RigidBodyIndex == selfRigidBodyIndex)
                            continue;

                        var e = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;

                        if (!volumeEntities.Contains(e))
                            continue;

                      //  FSLog.Info($"TriggerEntitiesSystem,entity:{e}");
                        if (!EntityManager.HasComponent<CatchFirePredictedState>(e))
                            continue;


                        var catchFireState = EntityManager.GetComponentData<CatchFirePredictedState>(e);
                        if (!catchFireState.IsCatchFire)
                            continue;

                        FSLog.Info($"TriggerEntitiesSystem,CatchFire,entity:{e}");
                        catchFireState.IsCatchFire = false;
                        EntityManager.SetComponentData(e, catchFireState);
                    }

                    distanceHits.Dispose();
                })
                //   .WithDeallocateOnJobCompletion(volumeEntities)
                .Run();
            volumeEntities.Dispose();
        }
    }
}