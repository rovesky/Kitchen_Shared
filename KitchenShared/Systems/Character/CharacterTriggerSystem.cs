using FootStone.ECS;
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
    public class CharacterTriggerSystem : JobComponentSystem
    {
        private BuildPhysicsWorld m_BuildPhysicsWorldSystem;

        private EntityQuery m_CharacterControllersGroup;
      //  private EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;
        private ExportPhysicsWorld m_ExportPhysicsWorldSystem;
      //  private EntityQuery m_OverlappingGroup;
       private EntityQuery m_TriggerVolumeGroup;

       protected override void OnCreate()
       {
           m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
           m_ExportPhysicsWorldSystem = World.GetOrCreateSystem<ExportPhysicsWorld>();

           var query = new EntityQueryDesc
           {
               All = new ComponentType[]
               {
                   typeof(ServerEntity),
                   typeof(PhysicsCollider),
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

           var ccJob = new GetTriggerOverlappingJob
           {
               Entities = entities,
               PhysicsColliderGroup = physicsColliderGroup,
               PredictedStateGroup = predictedStateGroup,

               PhysicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld,
               VolumeEntities = m_TriggerVolumeGroup.ToEntityArray(Allocator.TempJob),
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
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> VolumeEntities;


            public unsafe void Execute()
            {
              
                var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);

                foreach (var entity in Entities)
                {
                    var collider = PhysicsColliderGroup[entity];
                    var predictedState = PredictedStateGroup[entity];
                    // Collision filter must be valid
                    Assert.IsTrue(collider.ColliderPtr->Filter.IsValid);

                    // Character transform
                    var transform = new RigidTransform
                    {
                        pos = predictedState.Position,
                        rot = predictedState.Rotation
                    };

                    var input = new ColliderDistanceInput
                    {
                        MaxDistance = 0.7f,
                        Transform = transform,
                        Collider = collider.ColliderPtr
                    };

                    var selfRigidBodyIndex = PhysicsWorld.GetRigidBodyIndex(entity);

                    distanceHits.Clear();
                    PhysicsWorld.CalculateDistance(input, ref distanceHits);

                    var triggerIndex = CheckTrigger(PhysicsWorld, VolumeEntities,
                        selfRigidBodyIndex,distanceHits);

                    predictedState.TriggerEntity = triggerIndex < 0 ?
                        Entity.Null : 
                        PhysicsWorld.Bodies[distanceHits[triggerIndex].RigidBodyIndex].Entity;
                
                    PredictedStateGroup[entity] = predictedState;
                }
                distanceHits.Dispose();
            }
        }

        //private struct AddNewOverlappingJob : IJob
        //{
        //    public EntityCommandBuffer CommandBuffer;

        //    [NativeFixedLength(1)] [ReadOnly] public NativeArray<int> TriggerEntitiesCount;
        //    [ReadOnly] public NativeArray<int> Characters;
        //    [ReadOnly] public NativeArray<Entity> TriggerEntities;
        //    [ReadOnly] public ComponentDataFromEntity<OverlappingTrigger> OverlappingGroup;
        //    [ReadOnly] public ComponentDataFromEntity<TriggerData> TriggerDataGroup;

        //    public void Execute()
        //    {
        //        for (var i = 0; i < TriggerEntitiesCount[0]; i++)
        //        {
        //            var overlappingEntity = TriggerEntities[i];
        //            if (!OverlappingGroup.Exists(overlappingEntity))
        //            {
        //                var triggerComponent = TriggerDataGroup[overlappingEntity];
        //                CommandBuffer.AddComponent(overlappingEntity,
        //                    new OverlappingTrigger {TriggerEntityIndex = Characters[i]});
        //                CommandBuffer.AddComponent(overlappingEntity, new OnTriggerEnter());
        //                switch ((TriggerVolumeType) triggerComponent.VolumeType)
        //                {
        //                    //TODO 根据不同的trigger添加不同的组件
        //                    case TriggerVolumeType.None:
        //                    default:
        //                        break;
        //                }
        //            }
        //            else
        //            {
        //                var overlappingTriggerComponent = OverlappingGroup[overlappingEntity];
        //                if (overlappingTriggerComponent.TriggerEntityIndex != Characters[i])
        //                {
        //                    overlappingTriggerComponent.TriggerEntityIndex = Characters[i];
        //                    CommandBuffer.SetComponent(overlappingEntity, overlappingTriggerComponent);
        //                }
        //            }
        //        }
        //    }
        //}

        //private struct RemoveOldOverlappingJob : IJob
        //{
        //    public EntityCommandBuffer CommandBuffer;

        //    [NativeFixedLength(1)] [ReadOnly] public NativeArray<int> TriggerEntitiesCount;
        //    [ReadOnly] public NativeArray<Entity> TriggerEntities;
        //    [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> OverlappingEntities;
        //    [ReadOnly] public ComponentDataFromEntity<TriggerData> TriggerDataGroup;

        //    public void Execute()
        //    {
        //        for (var index = 0; index < OverlappingEntities.Length; index++)
        //        {
        //            var entity = OverlappingEntities[index];

        //            var isTriggered = false;
        //            for (var i = 0; i < TriggerEntitiesCount[0]; ++i)
        //                if (TriggerEntities[i] == entity)
        //                {
        //                    isTriggered = true;
        //                    break;
        //                }

        //            if (!isTriggered)
        //            {
        //                var triggerComponent = TriggerDataGroup[entity];

        //                //TODO 根据类型删除对应组件
        //                switch ((TriggerVolumeType) triggerComponent.VolumeType)
        //                {
        //                    case TriggerVolumeType.None:
        //                    default:
        //                        break;
        //                }

        //                CommandBuffer.RemoveComponent<OverlappingTrigger>(entity);
        //                CommandBuffer.AddComponent(entity, new OnTriggerExit());
        //            }
        //        }
        //    }
        //}
    }
}