using FootStone.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using UnityEngine.Assertions;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterMoveSystem : JobComponentSystem
    {
        private BuildPhysicsWorld m_BuildPhysicsWorldSystem;
        //   private EndFramePhysicsSystem m_EndFramePhysicsSystem;

        private EntityQuery m_CharacterControllersGroup;
        private ExportPhysicsWorld m_ExportPhysicsWorldSystem;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            m_ExportPhysicsWorldSystem = World.GetOrCreateSystem<ExportPhysicsWorld>();
            //m_EndFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();

            var query = new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                 //   typeof(ServerEntity),
                    typeof(PhysicsCollider),
                    typeof(CharacterMove),
                    typeof(UserCommand),
                    typeof(CharacterPredictedState)
                }
            };
            m_CharacterControllersGroup = GetEntityQuery(query);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            //var chunks = m_CharacterControllersGroup.CreateArchetypeChunkArray(Allocator.TempJob);

            var physicsColliderType = GetArchetypeChunkComponentType<PhysicsCollider>();
            var characterMoveType = GetArchetypeChunkComponentType<CharacterMove>();
            var userCommandType = GetArchetypeChunkComponentType<UserCommand>();
            var predictDataType = GetArchetypeChunkComponentType<CharacterPredictedState>();
            var entityType = GetArchetypeChunkEntityType();
            var tickDuration = GetSingleton<WorldTime>().TickDuration;

            var ccJob = new CharacterMoveControlJob
            {
                //	Chunks = chunks,
                // Archetypes
                PhysicsColliderType = physicsColliderType,
                CharacterMoveType = characterMoveType,
                UserCommandType = userCommandType,
                PredictDataType = predictDataType,
                EntitytType = entityType,
                // Input
                DeltaTime = tickDuration,
                PhysicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld
            };

            inputDeps = JobHandle.CombineDependencies(inputDeps, m_ExportPhysicsWorldSystem.FinalJobHandle);
            inputDeps = ccJob.Schedule(m_CharacterControllersGroup, inputDeps);

            return inputDeps;
        }

        [BurstCompile]
        private struct CharacterMoveControlJob : IJobChunk
        {
            // Chunks can be deallocated at this point
            //[DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> Chunks;

            public float DeltaTime;

            [ReadOnly] public PhysicsWorld PhysicsWorld;

            public ArchetypeChunkComponentType<CharacterPredictedState> PredictDataType;
            [ReadOnly] public ArchetypeChunkEntityType EntitytType;
            [ReadOnly] public ArchetypeChunkComponentType<PhysicsCollider> PhysicsColliderType;
            [ReadOnly] public ArchetypeChunkComponentType<CharacterMove> CharacterMoveType;
            [ReadOnly] public ArchetypeChunkComponentType<UserCommand> UserCommandType;
          
            public unsafe void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var chunkEntityData = chunk.GetNativeArray(EntitytType);
                var chunkPhysicsColliderData = chunk.GetNativeArray(PhysicsColliderType);
                var chunkCharacterMoveData = chunk.GetNativeArray(CharacterMoveType);
                var chunkUserCommandData = chunk.GetNativeArray(UserCommandType);
                var chunkPredictDataData = chunk.GetNativeArray(PredictDataType);
             
                const int maxQueryHits = 128;
                var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
                //var castHits = new NativeArray<ColliderCastHit>(maxQueryHits, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                var constraints = new NativeArray<SurfaceConstraintInfo>(4 * maxQueryHits, Allocator.Temp,
                    NativeArrayOptions.UninitializedMemory);

                for (var i = 0; i < chunk.Count; i++)
                {
                    var entity = chunkEntityData[i];
                    var collider = chunkPhysicsColliderData[i];
                    var characterMove = chunkCharacterMoveData[i];
                    var userCommand = chunkUserCommandData[i];
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

                    //	var selfRigidBodyIndex = PhysicsWorld.GetRigidBodyIndex(characterData.Entity);
                    var selfRigidBodyIndex = PhysicsWorld.GetRigidBodyIndex(entity);

                    PhysicsWorld.CalculateDistance(input, ref distanceHits);

                    var skinWidth = characterMove.SkinWidth;
                    CharacterControllerUtilities.CheckSupport(PhysicsWorld, selfRigidBodyIndex, skinWidth, distanceHits,
                        ref constraints, out var numConstraints);

                    float3 desiredVelocity = userCommand.targetPos * characterMove.Velocity;

                    // Solve
                    var newVelocity = desiredVelocity;
                    var newPosition = transform.pos;
                    var remainingTime = DeltaTime;
                    var up = math.up();
                    SimplexSolver.Solve(PhysicsWorld, remainingTime, up, numConstraints, ref constraints,
                        ref newPosition, ref newVelocity, out var integratedTime);

                  //  FSLog.Info($"newPosition:{newPosition.x},{newPosition.y},{newPosition.z}");
                    predictData.Position = newPosition;
                    if (math.distancesq(desiredVelocity, float3.zero) > 0.0001f)
                        predictData.Rotation = quaternion.LookRotationSafe(desiredVelocity, up);
                    chunkPredictDataData[i] = predictData;
                }
            }
        }
    }
}