using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class TriggerSystem : JobComponentSystem
    {
        private BuildPhysicsWorld m_BuildPhysicsWorldSystem;
        private KitchenExportPhysicsWorld m_ExportPhysicsWorldSystem;
        private EntityQuery m_TriggerVolumeGroup;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            m_ExportPhysicsWorldSystem = World.GetOrCreateSystem<KitchenExportPhysicsWorld>();
            m_TriggerVolumeGroup = GetEntityQuery(typeof(TriggerData));
        }

        protected override unsafe JobHandle OnUpdate(JobHandle inputDeps)
        {
            inputDeps = JobHandle.CombineDependencies(inputDeps, m_ExportPhysicsWorldSystem.FinalJobHandle);
            var volumeEntities = m_TriggerVolumeGroup.ToEntityArray(Allocator.TempJob);
            var physicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld;

            Entities.ForEach((Entity entity, ref TriggerPredictedState predictedState,
                in TriggerSetting setting, in EntityPredictedState entityState, in PhysicsCollider collider) =>
            {
                var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

                // Character transform
                var transform = entityState.Transform;

                var input = new ColliderDistanceInput
                {
                    MaxDistance = setting.Distance,
                    Transform = transform,
                    Collider = collider.ColliderPtr
                };

                var selfRigidBodyIndex = physicsWorld.GetRigidBodyIndex(entity);
                physicsWorld.CalculateDistance(input, ref distanceHits);

                // if (volumeEntities.Length > 0)
                //FSLog.Info($"volumeEntities.Length:{volumeEntities.Length}");

                var triggerIndex = CheckTrigger(ref physicsWorld, ref volumeEntities,
                    selfRigidBodyIndex, ref distanceHits);

                predictedState.TriggeredEntity = triggerIndex < 0
                    ? Entity.Null
                    : physicsWorld.Bodies[distanceHits[triggerIndex].RigidBodyIndex].Entity;

                // if (predictedState.TriggeredEntity != Entity.Null)
                //  FSLog.Info($"triggerEntity:{predictedState.TriggeredEntity}");

                distanceHits.Dispose();
            }).Schedule(inputDeps).Complete();

            volumeEntities.Dispose();
            return inputDeps;
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