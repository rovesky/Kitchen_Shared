using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterExtinguishingSystem : SystemBase
    {
        private KitchenBuildPhysicsWorld m_BuildPhysicsWorldSystem;
        private EntityQuery m_TriggerVolumeGroup;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<KitchenBuildPhysicsWorld>();
            m_TriggerVolumeGroup = GetEntityQuery(typeof(CatchFire));
        }

        protected override void OnUpdate()
        {

         
            Entities
                .WithAll<ServerEntity>()
                .WithStructuralChanges()
            //    .WithReadOnly(volumeEntities)
                .ForEach((in SlotPredictedState slotState,
                    in TransformPredictedState transformState) =>
                {
                    if (slotState.FilledIn == Entity.Null)
                        return;

                    if (!EntityManager.HasComponent<Extinguisher>(slotState.FilledIn))
                        return;

                    var extinguisherEntity = slotState.FilledIn;
                    var extinguisherState =
                        EntityManager.GetComponentData<ExtinguisherPredictedState>(extinguisherEntity);

                    if (extinguisherState.Distance == 0)
                        return;

                    var pos = transformState.Position + math.forward(transformState.Rotation) * 1.8f;
                    var rot = transformState.Rotation;
                    pos.y = 1.0f;
                    var input = new RaycastInput
                    {
                        Start = pos,
                        End = pos + math.forward(rot) * extinguisherState.Distance,
                        Filter = CollisionFilter.Default
                    };

                    ref var physicsWorld = ref m_BuildPhysicsWorldSystem.PhysicsWorld;

                    var raycastHits = new NativeList<RaycastHit>(Allocator.Temp);
                    if (!physicsWorld.CastRay(input, ref raycastHits))
                        return;


                    var volumeEntities = m_TriggerVolumeGroup.ToEntityArray(Allocator.TempJob);
                    if (volumeEntities.Length == 0)
                    {
                        volumeEntities.Dispose();
                        return;
                    }
               

                    for (var i = 0; i < raycastHits.Length; i++)
                    {
                        var hit = raycastHits[i];
                        var e = physicsWorld.Bodies[hit.RigidBodyIndex].Entity;

                        if (!volumeEntities.Contains(e))
                            continue;

                        //灭火
                        var catchFireSetting = EntityManager.GetComponentData<CatchFireSetting>(e);
                        var catchFireState = EntityManager.GetComponentData<CatchFirePredictedState>(e);

                        catchFireState.CurExtinguishTick++;
                        if (catchFireState.CurExtinguishTick >= catchFireSetting.TotalExtinguishTick)
                        {
                            catchFireState.IsCatchFire = false;
                            catchFireState.CurCatchFireTick = 0;
                            catchFireState.CurExtinguishTick = 0;
                        }

                        EntityManager.SetComponentData(e, catchFireState);
                    }
                    volumeEntities.Dispose();
                    raycastHits.Dispose();
                })
                //   .WithDeallocateOnJobCompletion(volumeEntities)
                .Run();
            
        }
    }
}