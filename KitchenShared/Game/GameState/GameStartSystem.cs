using System;
using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class GameStartSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .ForEach((Entity entity,
                    ref Countdown countdown,
                    ref GameStateComponent gameState) =>
                {
                    if(gameState.State != GameState.Preparing )
                        return;

                    if(countdown.Value != 0)
                        return;

                    var now = DateTime.Now;
                    gameState.State = GameState.Playing;
                    gameState.StartTime = now.Ticks;
                    countdown.Value = 10;
                    countdown.EndTime = now.AddSeconds(countdown.Value).Ticks;

                    FSLog.Info($"GameStart,countdown:{countdown.Value}!");

                }).Run();
        }
    }
}