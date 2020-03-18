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
    public class KitchenStepPhysicsWorld : JobComponentSystem
    {
        public JobHandle FinalJobHandle { get; private set; }

        private BuildPhysicsWorld m_BuildPhysicsWorldSystem;
        private SimulationContext SimulationContext;
     
        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();

            SimulationContext = new SimulationContext();
            SimulationContext.Reset(ref m_BuildPhysicsWorldSystem.PhysicsWorld);

        }

        protected override void OnDestroy()
        {
            SimulationContext.Dispose();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var handle = JobHandle.CombineDependencies(inputDeps, m_BuildPhysicsWorldSystem.FinalJobHandle);
           // var handle = JobHandle.CombineDependencies(inputDeps, m_StepPhysicsWorldSystem.FinalJobHandle);

            ref PhysicsWorld world = ref m_BuildPhysicsWorldSystem.PhysicsWorld;
#if !UNITY_DOTSPLAYER
            float timeStep = UnityEngine.Time.fixedDeltaTime;
#else
        float timeStep = Time.DeltaTime;
#endif
            var stepInput = new SimulationStepInput
            {
                World = world,
                TimeStep = timeStep,
                NumSolverIterations = PhysicsStep.Default.SolverIterationCount,
                Gravity = PhysicsStep.Default.Gravity,
                SynchronizeCollisionWorld = true,
            };

            handle = world.CollisionWorld.ScheduleUpdateDynamicTree(
                ref world, stepInput.TimeStep, stepInput.Gravity, handle);
        
            // NOTE: Currently the advice is to not chain local simulation steps.
            // Therefore we complete necessary work here and at each step.
            handle.Complete();

            SimulationContext.Reset(ref stepInput.World);

            Simulation.StepImmediate(stepInput, ref SimulationContext);


            FinalJobHandle = handle;
            return handle;
        }
    
    }
}
