using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 更新飞行状态
    /// </summary>
    [DisableAutoCreation]
    public class UpdateFlyingStateSystem : SystemBase
    {  private KitchenBuildPhysicsWorld m_BuildPhysicsWorldSystem;
   
        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.GetExistingSystem<KitchenBuildPhysicsWorld>();
        }

        protected override unsafe void OnUpdate()
        {
            var physicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld;
       
            Dependency = Entities.WithAll<ServerEntity>()
                .ForEach((Entity entity,
                    ref FlyingPredictedState flyingState,
                    in TransformPredictedState transformState,
                    in OwnerPredictedState ownerState,
                    in PhysicsCollider collider) =>
                {
                    if (!flyingState.IsFlying)
                        return;

                    var selfRigidBodyIndex = physicsWorld.GetRigidBodyIndex(entity);
                    // Query the world
                    var distanceHits =
                        new NativeList<DistanceHit>(8, Allocator.Temp);
                    var distanceHitsCollector =
                        new CharacterMoveUtilities.SelfFilteringAllHitsCollector<DistanceHit>(
                            selfRigidBodyIndex,0.1f, ref distanceHits);
                    var input = new ColliderDistanceInput
                    {
                        MaxDistance = 0.1f,
                        Transform = new RigidTransform()
                        {
                            pos = transformState.Position,
                            rot = transformState.Rotation
                        },
                        Collider = collider.ColliderPtr
                    };
                    physicsWorld.CalculateDistance(input, ref distanceHitsCollector);

                    if(distanceHitsCollector.NumHits == 0)
                        return;

                    for (var i = 0; i < distanceHitsCollector.AllHits.Length; i++)
                    {
                        var hit = distanceHitsCollector.AllHits[i];
                        var e = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;
                       // FSLog.Info($"flyingState trigger entity:{e}");
                        if (e == ownerState.PreOwner)
                            return;
                    }

                    flyingState.IsFlying = false;
                   // FSLog.Info($"flyingState.IsFlying = false,entity:{entity}");

                }).ScheduleParallel(Dependency);

            CompleteDependency();
        }
    }
}