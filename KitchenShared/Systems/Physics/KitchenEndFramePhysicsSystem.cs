using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics.Systems;

namespace FootStone.Kitchen
{
    // A system which combines the dependencies of all other physics jobs created during this frame into a single handle,
    // so that any system which depends on all physics work to be finished can just depend on this single handle.
    [DisableAutoCreation]
    public class KitchenEndFramePhysicsSystem : JobComponentSystem
    {
        // Extra physics jobs added by user systems
        public NativeList<JobHandle> HandlesToWaitFor;

        // A combined handle of all built-in and user physics jobs
        public JobHandle FinalJobHandle { get; private set; }

        KitchenBuildPhysicsWorld m_BuildPhysicsWorld;
       // StepPhysicsWorld m_StepPhysicsWorld;
        KitchenStepPhysicsWorld kitchenStepPhysicsWorldSystem;

        KitchenExportPhysicsWorld m_ExportPhysicsWorld;

        JobHandle CombineDependencies()
        {
            // Add built-in jobs
            HandlesToWaitFor.Add(m_BuildPhysicsWorld.FinalJobHandle);
         //   HandlesToWaitFor.Add(m_StepPhysicsWorld.FinalJobHandle);
            HandlesToWaitFor.Add(kitchenStepPhysicsWorldSystem.FinalJobHandle);
            HandlesToWaitFor.Add(m_ExportPhysicsWorld.FinalJobHandle);
            var handle = JobHandle.CombineDependencies(HandlesToWaitFor);
            HandlesToWaitFor.Clear();
            return handle;
        }

        protected override void OnCreate()
        {
            HandlesToWaitFor = new NativeList<JobHandle>(16, Allocator.Persistent);
            FinalJobHandle = new JobHandle();

            m_BuildPhysicsWorld = World.GetOrCreateSystem<KitchenBuildPhysicsWorld>();
          //  m_StepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
            kitchenStepPhysicsWorldSystem = World.GetOrCreateSystem<KitchenStepPhysicsWorld>();
            m_ExportPhysicsWorld = World.GetOrCreateSystem<KitchenExportPhysicsWorld>();
        }

        protected override void OnDestroy()
        {
            CombineDependencies().Complete();
            HandlesToWaitFor.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            FinalJobHandle = JobHandle.CombineDependencies(CombineDependencies(), inputDeps);
            return FinalJobHandle;
        }
    }
}
