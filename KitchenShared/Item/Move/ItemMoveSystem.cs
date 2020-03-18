//using FootStone.ECS;
//using Unity.Burst;
//using Unity.Collections;
//using Unity.Entities;
//using Unity.Jobs;
//using Unity.Mathematics;
//using Unity.Physics;
//using Unity.Physics.Systems;
//using UnityEngine.Assertions;

//namespace FootStone.Kitchen
//{
//    [DisableAutoCreation]
//    public class ItemMoveSystem : JobComponentSystem
//    {
//        private EntityQuery itemsGroup;
//        private BuildPhysicsWorld m_BuildPhysicsWorldSystem;
//        private KitchenExportPhysicsWorld m_ExportPhysicsWorldSystem;

//        protected override void OnCreate()
//        {
//            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
//            m_ExportPhysicsWorldSystem = World.GetOrCreateSystem<KitchenExportPhysicsWorld>();

//            var query = new EntityQueryDesc
//            {
//                All = new ComponentType[]
//                {
//                    typeof(ServerEntity),
//                    typeof(PhysicsCollider),
//                    typeof(TransformPredictedState)
//                },
//                None = new ComponentType[]
//                {
//                    typeof(Character),
//                }
//            };
//            itemsGroup = GetEntityQuery(query);
//        }


//        protected override JobHandle OnUpdate(JobHandle inputDeps)
//        {
//            var physicsColliderType = GetArchetypeChunkComponentType<PhysicsCollider>();
//            var transformPredictDataType = GetArchetypeChunkComponentType<TransformPredictedState>();
//            var velocityPredictedDataType = GetArchetypeChunkComponentType<VelocityPredictedState>();
//            var entityType = GetArchetypeChunkEntityType();
//            var tickDuration = GetSingleton<WorldTime>().TickDuration;

//            var ccJob = new CharacterMoveControlJob
//            {
//                // Archetypes
//                PhysicsColliderType = physicsColliderType,
//                TransformPredictDataType = transformPredictDataType,
//                VelocityPredictDataType = velocityPredictedDataType,
//                EntityType = entityType,
//                // Input
//                DeltaTime = tickDuration,
//                PhysicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld
//            };

//            inputDeps = JobHandle.CombineDependencies(inputDeps, m_ExportPhysicsWorldSystem.FinalJobHandle);
//            inputDeps = ccJob.Schedule(itemsGroup, inputDeps);

//            return inputDeps;
//        }

//        [BurstCompile]
//        private struct CharacterMoveControlJob : IJobChunk
//        {
//            public float DeltaTime;

//            [ReadOnly] public PhysicsWorld PhysicsWorld;

//            public ArchetypeChunkComponentType<TransformPredictedState> TransformPredictDataType;
//            public ArchetypeChunkComponentType<VelocityPredictedState> VelocityPredictDataType;
//            [ReadOnly] public ArchetypeChunkEntityType EntityType;
//            [ReadOnly] public ArchetypeChunkComponentType<PhysicsCollider> PhysicsColliderType;


//            public unsafe void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
//            {
//                var chunkEntityData = chunk.GetNativeArray(EntityType);
//                var chunkPhysicsColliderData = chunk.GetNativeArray(PhysicsColliderType);
//                var chunkTransformPredictData = chunk.GetNativeArray(TransformPredictDataType);
//                var chunkVelocityPredictData = chunk.GetNativeArray(VelocityPredictDataType);

//                for (var i = 0; i < chunk.Count; i++)
//                {
//                    var entity = chunkEntityData[i];
//                    var collider = chunkPhysicsColliderData[i];
//                    var transformPredictData = chunkTransformPredictData[i];
//                    var velocityPredictData = chunkVelocityPredictData[i];

//                    // Collision filter must be valid
//                    Assert.IsTrue(collider.ColliderPtr->Filter.IsValid);

//                    // Character transform
//                    var transform = new RigidTransform
//                    {
//                        pos = transformPredictData.Position,
//                        rot = transformPredictData.Rotation
//                    };

//                    var newVelocity = velocityPredictData.Linear + PhysicsStep.Default.Gravity * DeltaTime;
//                    var newPosition = transform.pos;

//                    ItemMoveUtilities.CollideAndIntegrate(ref PhysicsWorld, 0.1f, 0.5f, 11.0f,
//                        collider.ColliderPtr, DeltaTime, transform, math.up(), entity, ref newPosition,
//                        ref newVelocity);


//                    transformPredictData.Position = newPosition;
//                    velocityPredictData.Linear = newVelocity;
//                    chunkTransformPredictData[i] = transformPredictData;
//                    chunkVelocityPredictData[i] = velocityPredictData;
//                }
//            }
//        }
//    }
//}