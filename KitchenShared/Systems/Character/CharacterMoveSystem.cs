//using FootStone.ECS;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Physics.Extensions;
//using Unity.Physics.Systems;
//using UnityEngine;
//using UnityEngine.Assertions;
//using Math = System.Math;

//namespace FootStone.Kitchen
//{
//    [DisableAutoCreation]
//    public class CharacterMoveSystem : JobComponentSystem
//    {
//        private BuildPhysicsWorld m_BuildPhysicsWorldSystem;
   
//        private EntityQuery m_CharacterControllersGroup;
//        private MyExportPhysicsWorld m_ExportPhysicsWorldSystem;

//        protected override void OnCreate()
//        {
//            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
//            m_ExportPhysicsWorldSystem = World.GetOrCreateSystem<MyExportPhysicsWorld>();
     
//            var query = new EntityQueryDesc
//            {
//                All = new ComponentType[]
//                {
//                    typeof(ServerEntity),
//                    typeof(PhysicsCollider),
//                    typeof(CharacterMove),
//                    typeof(UserCommand),
//                    typeof(CharacterPredictedState)
//                }
//            };
//            m_CharacterControllersGroup = GetEntityQuery(query);
//        }

//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            var physicsColliderType = GetArchetypeChunkComponentType<PhysicsCollider>();
//            var characterMoveType = GetArchetypeChunkComponentType<CharacterMove>();
//            var userCommandType = GetArchetypeChunkComponentType<UserCommand>();
//            var predictDataType = GetArchetypeChunkComponentType<CharacterPredictedState>();
//            var entityType = GetArchetypeChunkEntityType();
//            var tickDuration = GetSingleton<WorldTime>().TickDuration;

//            var ccJob = new CharacterMoveControlJob
//            {
//                // Archetypes
//                PhysicsColliderType = physicsColliderType,
//                CharacterMoveType = characterMoveType,
//                UserCommandType = userCommandType,
//                PredictDataType = predictDataType,
//                EntityType = entityType,
//                // Input
//                DeltaTime = tickDuration,
//                PhysicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld
//            };

//            inputDeps = JobHandle.CombineDependencies(inputDeps, m_ExportPhysicsWorldSystem.FinalJobHandle);
//            inputDeps = ccJob.Schedule(m_CharacterControllersGroup, inputDeps);

//            return inputDeps;
//        }

//        [BurstCompile]
//        private struct CharacterMoveControlJob : IJobChunk
//        {
//            public float DeltaTime;

//            [ReadOnly] public PhysicsWorld PhysicsWorld;

//            public ArchetypeChunkComponentType<CharacterPredictedState> PredictDataType;
//            [ReadOnly] public ArchetypeChunkEntityType EntityType;
//            [ReadOnly] public ArchetypeChunkComponentType<PhysicsCollider> PhysicsColliderType;
//            [ReadOnly] public ArchetypeChunkComponentType<CharacterMove> CharacterMoveType;
//            [ReadOnly] public ArchetypeChunkComponentType<UserCommand> UserCommandType;
          
//            public unsafe void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
//            {
//                var chunkEntityData = chunk.GetNativeArray(EntityType);
//                var chunkPhysicsColliderData = chunk.GetNativeArray(PhysicsColliderType);
//                var chunkCharacterMoveData = chunk.GetNativeArray(CharacterMoveType);
//                var chunkUserCommandData = chunk.GetNativeArray(UserCommandType);
//                var chunkPredictDataData = chunk.GetNativeArray(PredictDataType);

//                for (var i = 0; i < chunk.Count; i++)
//                {
//                    var entity = chunkEntityData[i];
//                    var collider = chunkPhysicsColliderData[i];
//                    var characterMove = chunkCharacterMoveData[i];
//                    var userCommand = chunkUserCommandData[i];
//                    var predictData = chunkPredictDataData[i];

//                    // Collision filter must be valid
//                    Assert.IsTrue(collider.ColliderPtr->Filter.IsValid);

//                    // Character transform
//                    var transform = new RigidTransform
//                    {
//                        pos = predictData.Position,
//                        rot = predictData.Rotation
//                    };

//                    var input = new ColliderDistanceInput
//                    {
//                        MaxDistance = 0.5f,
//                        Transform = transform,
//                        Collider = collider.ColliderPtr
//                    };

//                    var selfRigidBodyIndex = PhysicsWorld.GetRigidBodyIndex(entity);
//                    var distanceHits = new NativeList<DistanceHit>(8, Allocator.Temp);
//                    var constraints = new NativeList<SurfaceConstraintInfo>(16, Allocator.Temp);

//                    PhysicsWorld.CalculateDistance(input, ref distanceHits);

//                    var skinWidth = characterMove.SkinWidth;
//                    CharacterControllerUtilities.CreateConstraints(PhysicsWorld, selfRigidBodyIndex, skinWidth,
//                        ref distanceHits,
//                        ref constraints);
//                    //  FSLog.Info($"targetPos:{userCommand.targetPos.x},{userCommand.targetPos.y},{userCommand.targetPos.z}");
//                    predictData.LinearVelocity =
//                        transform.pos.y > 1.3f ? predictData.LinearVelocity+ PhysicsStep.Default.Gravity * DeltaTime : float3.zero;

//                    FSLog.Info($"gravityVelocity:{predictData.LinearVelocity},transform.pos{transform.pos}");
//                    var desiredVelocity = (float3) userCommand.TargetDir * characterMove.Velocity;

//                    // Solve
//                    var newVelocity = desiredVelocity + predictData.LinearVelocity;
//                    var newPosition = transform.pos;
                 
//                    var remainingTime = DeltaTime;
//                    var up = math.up();
//                    SimplexSolver.Solve(PhysicsWorld, remainingTime,
//                        remainingTime, up, characterMove.Velocity,
//                        constraints, ref newPosition, ref newVelocity, out var integratedTime);


//                    //旋转角度计算
//                    if (math.distancesq(desiredVelocity, float3.zero) > 0.0001f)
//                    {
//                        var fromRotation = predictData.Rotation;
//                        var toRotation = quaternion.LookRotationSafe(desiredVelocity, up);
//                        var angle = Quaternion.Angle(fromRotation, toRotation);
//                        predictData.Rotation = Quaternion.RotateTowards(fromRotation, toRotation,
//                            Math.Abs(angle - 180.0f) < float.Epsilon ? -22.5f : 22.5f);
//                    }

//                    predictData.Position = newPosition;
//                    chunkPredictDataData[i] = predictData;
//                }
//            }
//        }
//    }
//}