using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 火势蔓延
    /// </summary>
    [DisableAutoCreation]
    public class FireSpreadSystem : SystemBase
    {
        private KitchenBuildPhysicsWorld m_BuildPhysicsWorldSystem;
      
        protected override void OnCreate()
        {
          //  FSLog.Info($"FireSpreadSystem OnCreate1");

            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<KitchenBuildPhysicsWorld>();
         //   FSLog.Info($"FireSpreadSystem OnCreate2");

        }

        protected override void OnUpdate()
        {
         //   FSLog.Info($"FireSpreadSystem OnUpdate");
          
            Entities.WithAll<CatchFire>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref CatchFirePredictedState myCatchFireState,
                    in CatchFireSetting myCatchFireSetting,
                    in LocalToWorld localToWorld,
                    in SlotSetting slotSetting) =>
                {

                    if(!myCatchFireState.IsCatchFire)
                        return;
                //    FSLog.Info($"myCatchFireState.CurCatchFireTick:{myCatchFireState.CurCatchFireTick}");
                    if (myCatchFireState.CurCatchFireTick < myCatchFireSetting.FireSpreadTick)
                    {
                        myCatchFireState.CurCatchFireTick++;
                        return;
                    }
                
                    ref var physicsWorld =  ref m_BuildPhysicsWorldSystem.PhysicsWorld;
                    var pos = localToWorld.Position + math.mul(localToWorld.Rotation, slotSetting.Pos);
                    var input = new PointDistanceInput
                    {
                        Position = pos,
                        MaxDistance = myCatchFireSetting.FireSpreadRadius,
                        Filter = CollisionFilter.Default
                    };

                   // FSLog.Info($"FireSpreadSystem,Position:{input.Position}");

                    var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
                    if (!physicsWorld.CalculateDistance(input, ref distanceHits))
                        return;

                   // FSLog.Info($"distanceHits:{distanceHits.Length}");

                    for (var i = 0; i < distanceHits.Length; i++)
                    {
                        var hit = distanceHits[i];
                        var e = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;

                        if (!EntityManager.HasComponent<CatchFirePredictedState>(e))
                            continue;

                        var catchFireState = EntityManager.GetComponentData<CatchFirePredictedState>(e);
                        if(catchFireState.IsCatchFire)
                            continue;

                        catchFireState.IsCatchFire = true;
                        catchFireState.CurCatchFireTick = 0;
                        EntityManager.SetComponentData(e,catchFireState);
                    }

                }).Run();
        }
    }
}