using FootStone.ECS;
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
    public class MyExportPhysicsWorld : JobComponentSystem
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
            JobHandle handle = JobHandle.CombineDependencies(inputDeps, m_BuildPhysicsWorldSystem.FinalJobHandle);

            ref PhysicsWorld world = ref m_BuildPhysicsWorldSystem.PhysicsWorld;
            var predictedStateType = GetArchetypeChunkComponentType<EntityPredictedState>();
          
            handle = new ExportDynamicBodiesJob
            {
                MotionVelocities = world.MotionVelocities,
                MotionDatas = world.MotionDatas,

                PredictedStateType = predictedStateType
            }.Schedule(m_BuildPhysicsWorldSystem.DynamicEntityGroup, handle);


            FinalJobHandle = handle;
            return handle;
        }

        [BurstCompile]
        internal struct ExportDynamicBodiesJob : IJobChunk
        {
            [ReadOnly] public NativeSlice<MotionVelocity> MotionVelocities;
            [ReadOnly] public NativeSlice<MotionData> MotionDatas;
       
            public ArchetypeChunkComponentType<EntityPredictedState> PredictedStateType;

            public void Execute(ArchetypeChunk chunk, int chunkIndex, int entityStartIndex)
            {

                var chunkPredictedStates = chunk.GetNativeArray(PredictedStateType);

                var numItems = chunkPredictedStates.Length;

             //   FSLog.Info($"chunk.Count:{chunk.Count},chunkPredictedStates.Length:{chunkPredictedStates.Length}");
                for (int i = 0, motionIndex = entityStartIndex; i < numItems; i++, motionIndex++)
                {
                    var md = MotionDatas[motionIndex];
                    var worldFromBody = math.mul(md.WorldFromMotion, math.inverse(md.BodyFromMotion));

                    var predictState = chunkPredictedStates[i];
                    predictState.Transform.pos = worldFromBody.pos;
                    predictState.Transform.rot = worldFromBody.rot;
                    predictState.Velocity.Linear = MotionVelocities[motionIndex].LinearVelocity;
                    predictState.Velocity.Angular = MotionVelocities[motionIndex].AngularVelocity;
                    chunkPredictedStates[i] = predictState;
                
                }
            }
        }
    }
}
