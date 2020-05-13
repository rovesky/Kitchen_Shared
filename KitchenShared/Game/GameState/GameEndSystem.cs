using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class GameEndSystem : SystemBase
    {
        private EntityQuery spawnPointQuery;

        protected override void OnCreate()
        {
            spawnPointQuery = EntityManager.CreateEntityQuery(typeof(SpawnPoint));

        }

        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref Countdown countdown,
                    ref GameStateComponent gameState,
                    ref Score score) =>
                {
                    if(gameState.State != GameState.Playing )
                        return;

                    if(countdown.Value != 0)
                        return;

                    //状态重置
                    gameState.State = GameState.Ending;
                    score.Value = 0;
                    countdown.SetValue(0);

                    //清理menu
                    var menuQuery = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(MenuItem)
                        }
                    });
                    var menuEntities = menuQuery.ToEntityArray(Allocator.TempJob);
                    foreach (var menu in menuEntities)
                    {
                        EntityManager.SetComponentData(menu,new DespawnPredictedState()
                        {
                            IsDespawn = true,
                            Tick = 0
                        });
                    }

                    menuEntities.Dispose();

                    //清理slot
                    var slotQuery = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(SlotPredictedState)
                        }
                    });
                    var slotEntities = slotQuery.ToEntityArray(Allocator.TempJob);
                    foreach (var slotEntity in slotEntities)
                    {
                        var slot = EntityManager.GetComponentData<SlotPredictedState>(slotEntity);
                        slot.FilledIn = Entity.Null;
                        EntityManager.SetComponentData(slotEntity, slot);
                    }
                    slotEntities.Dispose();

                    //清理sink
                    var sinkQuery = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(SinkPredictedState)
                        }
                    });
                    var sinkEntities = sinkQuery.ToEntityArray(Allocator.TempJob);
                    foreach (var sinEntity in sinkEntities)
                    {
                        var slot = EntityManager.GetComponentData<SinkPredictedState>(sinEntity);
                        slot.Value.Clear();
                        EntityManager.SetComponentData(sinEntity, slot);
                    }
                    sinkEntities.Dispose();

                    
                    //清理multiSlot
                    var multiSlotQuery = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(MultiSlotPredictedState)
                        }
                    });
                    var multiSlotQueryEntities = multiSlotQuery.ToEntityArray(Allocator.TempJob);
                    foreach (var multiSlotEntity in multiSlotQueryEntities)
                    {
                        var slot = EntityManager.GetComponentData<MultiSlotPredictedState>(multiSlotEntity);
                        slot.Value.Clear();
                        EntityManager.SetComponentData(multiSlotEntity, slot);
                    }
                    multiSlotQueryEntities.Dispose();

                       
                    //清理CatchFire
                    var catchFireQuery = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(CatchFirePredictedState)
                        }
                    });
                    var catchFireEntities = catchFireQuery.ToEntityArray(Allocator.TempJob);
                    foreach (var catchFireEntity in catchFireEntities)
                    {
                        var catchFire = EntityManager.GetComponentData<CatchFirePredictedState>(catchFireEntity);
                        catchFire.CurCatchFireTick = 0;
                        catchFire.CurExtinguishTick = 0;
                        catchFire.IsCatchFire = false;
                        EntityManager.SetComponentData(catchFireEntity, catchFire);
                    }
                    catchFireEntities.Dispose();

                    //清理item
                    var itemQuery = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(Item)
                        }
                    });
                    var itemEntities = itemQuery.ToEntityArray(Allocator.TempJob);
                    foreach (var item in itemEntities)
                    {
                        EntityManager.SetComponentData(item,new DespawnPredictedState()
                        {
                            IsDespawn = true,
                            Tick = 0
                        });
                    }
                    itemEntities.Dispose();

                    //character归位
                    var characterQuery = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(Character)
                        }
                    });
                    var spawnPoints = spawnPointQuery.ToEntityArray(Allocator.TempJob);
                    var characterEntities = characterQuery.ToEntityArray(Allocator.TempJob);
                   
                    for ( var i = 0; i < characterEntities.Length; ++i)
                    {
                        var character = characterEntities[i];
                        var position = new float3(2, 1, -5);
                        if (i < spawnPoints.Length)
                            position = EntityManager.GetComponentData<LocalToWorld>(spawnPoints[i]).Position;
                    
                        var transform = EntityManager.GetComponentData<TransformPredictedState>(character);
                        transform.Position = position;
                        transform.Rotation = quaternion.identity;
                        EntityManager.SetComponentData(character,transform);

                        var velocity = EntityManager.GetComponentData<VelocityPredictedState>(character);
                        velocity.Angular = float3.zero;
                        velocity.Linear = float3.zero;
                        EntityManager.SetComponentData(character,velocity);

                        var move = EntityManager.GetComponentData<CharacterMovePredictedState>(character);
                        move.UnsupportedVelocity = float3.zero;
                        move.ImpulseVelocity = float3.zero;
                        move.ImpulseDuration = 0;
                        EntityManager.SetComponentData(character,move);

                        var slot = EntityManager.GetComponentData<SlotPredictedState>(character);
                        slot.FilledIn= Entity.Null;
                        EntityManager.SetComponentData(character,slot);

                        var trigger = EntityManager.GetComponentData<TriggerPredictedState>(character);
                        trigger.TriggeredEntity = Entity.Null;
                        EntityManager.SetComponentData(character,trigger);


                        var slice = EntityManager.GetComponentData<SlicePredictedState>(character);
                        slice.IsSlicing = false;
                        EntityManager.SetComponentData(character,slice);

                        var wash = EntityManager.GetComponentData<WashPredictedState>(character);
                        wash.IsWashing= false;
                        EntityManager.SetComponentData(character,wash);
                    }
                    spawnPoints.Dispose();
                    characterEntities.Dispose();
                  
                    FSLog.Info($"GameEnd!");

                }).Run();
        }
    }
}