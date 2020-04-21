using FootStone.ECS;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class GameEndSystem : SystemBase
    {
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
                        EntityManager.AddComponentData(menu, new Despawn());
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
                        EntityManager.AddComponentData(item, new Despawn());
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

                
                    var characterEntities = characterQuery.ToEntityArray(Allocator.TempJob);
                    foreach (var character in characterEntities)
                    {
                        var transform = EntityManager.GetComponentData<TransformPredictedState>(character);
                        transform.Position = new float3(2, 1, -5);
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
                    }
                    characterEntities.Dispose();

                    FSLog.Info($"GameEnd!");

                }).Run();
        }
    }
}