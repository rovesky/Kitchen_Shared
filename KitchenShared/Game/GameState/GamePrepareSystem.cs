using System;
using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class GamePrepareSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref GameStateComponent gameState,
                    ref Countdown countdown) =>
                {
                    if(gameState.State != GameState.Ending )
                        return;

                    var query = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(Connection)
                        }
                    });
                    if (query.CalculateEntityCount() == 0)
                        return;

                    gameState.State = GameState.Preparing;
                    countdown.Value = 10;
                    countdown.EndTime = DateTime.Now.AddSeconds(countdown.Value).Ticks;

                    
                    FSLog.Info($"Game Prepare");

                }).Run();
        }
    }
}