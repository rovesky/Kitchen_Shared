using System;
using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CountdownSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .ForEach((Entity entity,
                    ref Countdown countdown,
                    in GameStateComponent gameState) =>
                {
                   // FSLog.Info($"CountdownSystem，value：{ time.Value}");
                    if(gameState.State != GameState.Playing &&
                       gameState.State != GameState.Preparing )
                        return;

                    if(countdown.Value <= 0)
                        return;

                    var timeSpan = new DateTime(countdown.EndTime) - DateTime.Now;
                    countdown.Value = (ushort) timeSpan.TotalSeconds;
                //    FSLog.Info($"CountdownSystem，value：{ countdown.Value}");

                }).Run();
        }
    }
}