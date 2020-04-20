using FootStone.ECS;
using System;
using Unity.Entities;

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

                    gameState.State = GameState.Preparing;
                    score.Value = 0;
                    countdown.Value = 10;
                    countdown.EndTime = DateTime.Now.AddSeconds(countdown.Value).Ticks;

                    EntityManager.DestroyEntity(GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(Menu)
                        }
                    }));

                    FSLog.Info($"GameEnd!");

                }).Run();
        }
    }
}