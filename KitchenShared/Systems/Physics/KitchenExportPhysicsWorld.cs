using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;


namespace FootStone.Kitchen
{
    // A system which copies transforms and velocities from the physics world back to the original entity components.
    // CK: We make sure we update before CopyTransformToGameObjectSystem so that hybrid GameObjects can work with this OK, even if that path is slow.
    [DisableAutoCreation]
    public class KitchenExportPhysicsWorld : JobComponentSystem
    {
        public JobHandle FinalJobHandle { get; private set; }

        BuildPhysicsWorld m_BuildPhysicsWorldSystem;
        StepPhysicsWorld m_StepPhysicsWorldSystem;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            m_StepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorld>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
           // var handle = JobHandle.CombineDependencies(inputDeps, m_BuildPhysicsWorldSystem.FinalJobHandle);
            JobHandle handle = JobHandle.CombineDependencies(inputDeps, m_StepPhysicsWorldSystem.FinalJobHandle);

            ref PhysicsWorld world = ref m_BuildPhysicsWorldSystem.PhysicsWorld;
            var transformPredictedStateType = GetArchetypeChunkComponentType<TransformPredictedState>();
            var velocityPredictedStateType = GetArchetypeChunkComponentType<VelocityPredictedState>();

            handle = new ExportDynamicBodiesJob
            {
                MotionVelocities = world.MotionVelocities,
                MotionDatas = world.MotionDatas,

                TransformPredictedStateType = transformPredictedStateType,
                VelocityPredictedStateType = velocityPredictedStateType
            }.Schedule(m_BuildPhysicsWorldSystem.DynamicEntityGroup, handle);


            FinalJobHandle = handle;
            return handle;
        }

        [BurstCompile]
        internal struct ExportDynamicBodiesJob : IJobChunk
        {
            [ReadOnly] public NativeSlice<MotionVelocity> MotionVelocities;
            [ReadOnly] public NativeSlice<MotionData> MotionDatas;
       
            public ArchetypeChunkComponentType<TransformPredictedState> TransformPredictedStateType;
            public ArchetypeChunkComponentType<VelocityPredictedState> VelocityPredictedStateType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int entityStartIndex)
            {

                var chunkTransformPredictedStates = chunk.GetNativeArray(TransformPredictedStateType);
                var chunkVelocityPredictedStates = chunk.GetNativeArray(VelocityPredictedStateType);

                var numItems = chunkTransformPredictedStates.Length;

             //   FSLog.Info($"chunk.Count:{chunk.Count},chunkPredictedStates.Length:{chunkPredictedStates.Length}");
                for (int i = 0, motionIndex = entityStartIndex; i < numItems; i++, motionIndex++)
                {
                    var md = MotionDatas[motionIndex];
                    var worldFromBody = math.mul(md.WorldFromMotion, math.inverse(md.BodyFromMotion));

                    var transformPredictState = chunkTransformPredictedStates[i];
                    var velocityPredictState = chunkVelocityPredictedStates[i];


                    transformPredictState.Position = worldFromBody.pos;
                    transformPredictState.Rotation = worldFromBody.rot;
                    velocityPredictState.Linear = MotionVelocities[motionIndex].LinearVelocity;
                    velocityPredictState.Angular = MotionVelocities[motionIndex].AngularVelocity;
                    chunkTransformPredictedStates[i] = transformPredictState;
                    chunkVelocityPredictedStates[i] = velocityPredictState;

                }
            }
        }
    }
}
