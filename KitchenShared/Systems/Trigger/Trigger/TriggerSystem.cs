using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class TriggerSystem : SystemBase
    {
        private KitchenBuildPhysicsWorld m_BuildPhysicsWorldSystem;
   
        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.GetExistingSystem<KitchenBuildPhysicsWorld>();
         }

        protected override unsafe void OnUpdate()
        {
            var physicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld;
            Entities
                .WithAll<AllowTrigger>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref TriggerPredictedState triggerState,
                    in VelocityPredictedState velocityState) =>
                {
                    if (!triggerState.IsAllowTrigger ||
                        Vector3.SqrMagnitude(velocityState.Linear) < 0.001)
                        EntityManager.RemoveComponent<AllowTrigger>(entity);
               
                }).Run();

            Entities
                .WithNone<AllowTrigger>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref TriggerPredictedState triggerState,
                    in VelocityPredictedState velocityState) =>
                {
                    if (!triggerState.IsAllowTrigger)
                        return;

                    if (Vector3.SqrMagnitude(velocityState.Linear) > 0.001
                        || triggerState.TriggeredEntity == Entity.Null)
                        EntityManager.AddComponent<AllowTrigger>(entity);

                }).Run();


            Dependency = Entities
                .WithAll<ServerEntity, AllowTrigger, PhysicsVelocity>()
                //  .WithChangeFilter<TransformPredictedState>()
                .WithBurst()
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

                    //Debug.DrawLine(transformState.Position,
                    //    transformState.Position + math.mul(transformState.Rotation, Vector3.forward) * 1,
                    //    Color.red);

                    var selfRigidBodyIndex = physicsWorld.GetRigidBodyIndex(entity);
                    physicsWorld.CalculateDistance(input, ref distanceHits);

                    var triggerIndex = -1;
                    for (var i = 0; i < distanceHits.Length; i++)
                    {
                        var hit = distanceHits[i];
                        if (hit.RigidBodyIndex == selfRigidBodyIndex)
                            continue;

                        var e = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;

                        if(!HasComponent<TriggeredState>(e))
                            continue;

                        if (HasComponent<Item>(e) && HasComponent<Item>(entity))
                            continue;

                        if (triggerIndex < 0)
                            triggerIndex = i;
                        else if (distanceHits[triggerIndex].Distance > hit.Distance)
                            triggerIndex = i;
                    }

                    triggerState.TriggeredEntity = triggerIndex < 0
                        ? Entity.Null
                        : physicsWorld.Bodies[distanceHits[triggerIndex].RigidBodyIndex].Entity;

                    distanceHits.Dispose();
                })
           //     .WithDeallocateOnJobCompletion(triggeredEntities)
                // .Run();
                // triggeredEntities.Dispose();
                .ScheduleParallel(Dependency);
            CompleteDependency();
        }

        //private  int CheckTrigger(ref PhysicsWorld world, ref NativeArray<Entity> volumeEntities,
        //   Entity selfEntity, int selfRigidBodyIndex, ref NativeList<DistanceHit> distanceHits)
        //{
        //    var triggerIndex = -1;
        //    for (var i = 0; i < distanceHits.Length; i++)
        //    {
        //        var hit = distanceHits[i];
        //        if (hit.RigidBodyIndex == selfRigidBodyIndex)
        //            continue;

        //        var e = world.Bodies[hit.RigidBodyIndex].Entity;
                
        //        if(HasComponent<Item>(e) && HasComponent<Item>(selfEntity))
        //            continue;

        //        if (!volumeEntities.Contains(e))
        //            continue;

        //        if (triggerIndex < 0)
        //            triggerIndex = i;
        //        else if (distanceHits[triggerIndex].Distance > hit.Distance)
        //            triggerIndex = i;
        //    }

        //    return triggerIndex;
        //}
    }
}