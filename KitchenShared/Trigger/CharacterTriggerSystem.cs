using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using UnityEngine.Assertions;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterTriggerSystem : JobComponentSystem
    {
        private BuildPhysicsWorld m_BuildPhysicsWorldSystem;

        private EntityQuery m_CharacterControllersGroup;
        private KitchenExportPhysicsWorld m_ExportPhysicsWorldSystem;

        private EntityQuery m_TriggerVolumeGroup;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            m_ExportPhysicsWorldSystem = World.GetOrCreateSystem<KitchenExportPhysicsWorld>();

            var query = new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(ServerEntity),
                    typeof(PhysicsCollider),
                    typeof(EntityPredictedState),
                    typeof(CharacterPredictedState)
                }
            };
            m_CharacterControllersGroup = GetEntityQuery(query);
            m_TriggerVolumeGroup = GetEntityQuery(typeof(TriggerData));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var entities = m_CharacterControllersGroup.ToEntityArray(Allocator.TempJob);

            var physicsColliderGroup = GetComponentDataFromEntity<PhysicsCollider>(true);
            var predictedStateGroup = GetComponentDataFromEntity<CharacterPredictedState>();
            var entityPredictedStateGroup = GetComponentDataFromEntity<EntityPredictedState>();

            var ccJob = new GetTriggerOverlappingJob
            {
                Entities = entities,
                PhysicsColliderGroup = physicsColliderGroup,
                PredictedStateGroup = predictedStateGroup,
                EntityPredictedStateGroup = entityPredictedStateGroup,
                PhysicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld,
                VolumeEntities = m_TriggerVolumeGroup.ToEntityArray(Allocator.TempJob)
            };

            inputDeps = JobHandle.CombineDependencies(inputDeps, m_ExportPhysicsWorldSystem.FinalJobHandle);
            ccJob.Schedule(inputDeps).Complete();
         
            return inputDeps;
        }

        private static int CheckTrigger(PhysicsWorld world, NativeArray<Entity> volumeEntities, int selfRigidBodyIndex,
            NativeList<DistanceHit> distanceHits)
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

        private struct GetTriggerOverlappingJob : IJob
        {
            // Chunks can be deallocated at this point
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> Entities;
            [ReadOnly] public PhysicsWorld PhysicsWorld;
            [ReadOnly] public ComponentDataFromEntity<PhysicsCollider> PhysicsColliderGroup;
            public ComponentDataFromEntity<CharacterPredictedState> PredictedStateGroup;
            [ReadOnly] public ComponentDataFromEntity<EntityPredictedState> EntityPredictedStateGroup;

            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> VolumeEntities;


            public unsafe void Execute()
            {
                var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

                foreach (var entity in Entities)
                {
                    var collider = PhysicsColliderGroup[entity];
                    var predictedState = PredictedStateGroup[entity];
                    var entityPredictedState = EntityPredictedStateGroup[entity];
                    // Collision filter must be valid
                    Assert.IsTrue(collider.ColliderPtr->Filter.IsValid);

                    // Character transform
                    var transform = entityPredictedState.Transform;

                    var input = new ColliderDistanceInput
                    {
                        MaxDistance = 0.7f,
                        Transform = transform,
                        Collider = collider.ColliderPtr
                    };

                    var selfRigidBodyIndex = PhysicsWorld.GetRigidBodyIndex(entity);

                    distanceHits.Clear();
                    PhysicsWorld.CalculateDistance(input, ref distanceHits);

                    //  if (distanceHits.Length > 6)
                    //    FSLog.Info($"distanceHits.Length:{distanceHits.Length}");

                    var triggerIndex = CheckTrigger(PhysicsWorld, VolumeEntities,
                        selfRigidBodyIndex, distanceHits);

                    predictedState.TriggeredEntity = triggerIndex < 0
                        ? Entity.Null
                        : PhysicsWorld.Bodies[distanceHits[triggerIndex].RigidBodyIndex].Entity;

                    // if (predictedState.TriggeredEntity != Entity.Null)
                    //  FSLog.Info($"triggerEntity:{predictedState.TriggeredEntity}");

                    PredictedStateGroup[entity] = predictedState;
                }

                distanceHits.Dispose();
            }
        }
    }
}