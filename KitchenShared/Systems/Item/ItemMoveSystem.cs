using FootStone.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using Math = System.Math;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ItemMoveSystem : JobComponentSystem
    {
        private BuildPhysicsWorld m_BuildPhysicsWorldSystem;
   
        private EntityQuery itemsGroup;
        private MyExportPhysicsWorld m_ExportPhysicsWorldSystem;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            m_ExportPhysicsWorldSystem = World.GetOrCreateSystem<MyExportPhysicsWorld>();
     
            var query = new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(ServerEntity),
                    typeof(PhysicsCollider),
                    typeof(ItemPredictedState),
                  
                }
            };
            itemsGroup = GetEntityQuery(query);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var physicsColliderType = GetArchetypeChunkComponentType<PhysicsCollider>();
            var predictDataType = GetArchetypeChunkComponentType<ItemPredictedState>();
            var entityType = GetArchetypeChunkEntityType();
            var tickDuration = GetSingleton<WorldTime>().TickDuration;

            var ccJob = new CharacterMoveControlJob
            {
                // Archetypes
                PhysicsColliderType = physicsColliderType,
                PredictDataType = predictDataType,
                EntityType = entityType,
                // Input
                DeltaTime = tickDuration,
                PhysicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld
            };

            inputDeps = JobHandle.CombineDependencies(inputDeps, m_ExportPhysicsWorldSystem.FinalJobHandle);
            inputDeps = ccJob.Schedule(itemsGroup, inputDeps);

            return inputDeps;
        }

        [BurstCompile]
        private struct CharacterMoveControlJob : IJobChunk
        {
            public float DeltaTime;

            [ReadOnly] public PhysicsWorld PhysicsWorld;

            public ArchetypeChunkComponentType<ItemPredictedState> PredictDataType;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;
            [ReadOnly] public ArchetypeChunkComponentType<PhysicsCollider> PhysicsColliderType;
           
          
            public unsafe void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var chunkEntityData = chunk.GetNativeArray(EntityType);
                var chunkPhysicsColliderData = chunk.GetNativeArray(PhysicsColliderType);
                var chunkPredictDataData = chunk.GetNativeArray(PredictDataType);

                for (var i = 0; i < chunk.Count; i++)
                {
                    var entity = chunkEntityData[i];
                    var collider = chunkPhysicsColliderData[i];
                    var predictData = chunkPredictDataData[i];

                    // Collision filter must be valid
                    Assert.IsTrue(collider.ColliderPtr->Filter.IsValid);

                    // Character transform
                    var transform = new RigidTransform
                    {
                        pos = predictData.Position,
                        rot = predictData.Rotation
                    };

                    var input = new ColliderDistanceInput
                    {
                        MaxDistance = 0.5f,
                        Transform = transform,
                        Collider = collider.ColliderPtr
                    };

                    var selfRigidBodyIndex = PhysicsWorld.GetRigidBodyIndex(entity);
                    var distanceHits = new NativeList<DistanceHit>(8, Allocator.Temp);
                    var constraints = new NativeList<SurfaceConstraintInfo>(16, Allocator.Temp);

                    PhysicsWorld.CalculateDistance(input, ref distanceHits);

                 //   var skinWidth = characterMove.SkinWidth;
                    CharacterControllerUtilities.CreateConstraints(PhysicsWorld, selfRigidBodyIndex, 0.1f,
                        ref distanceHits,
                        ref constraints);
                    //  FSLog.Info($"targetPos:{userCommand.targetPos.x},{userCommand.targetPos.y},{userCommand.targetPos.z}");
                    
                    // Solve
                    var newVelocity = predictData.LinearVelocity + PhysicsStep.Default.Gravity * DeltaTime;;
                    var newPosition = transform.pos;
                    //   newPosition.y = 1.2f;
                    var remainingTime = DeltaTime;
                    var up = math.up();
                    SimplexSolver.Solve(PhysicsWorld, remainingTime,
                        remainingTime, up, 11.0f,
                        constraints, ref newPosition, ref newVelocity, out var integratedTime);

                    predictData.Position = newPosition;
                    predictData.LinearVelocity = newVelocity;
                    chunkPredictDataData[i] = predictData;
                }
            }
        }
    }
}