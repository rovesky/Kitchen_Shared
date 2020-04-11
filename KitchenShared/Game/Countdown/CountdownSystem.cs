using System;
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
                    ref Countdown time) =>
                {
                    if(time.Value <= 0)
                        return;

                    var timeSpan = new DateTime(time.EndTime) - DateTime.Now;
                    time.Value = (ushort) timeSpan.TotalSeconds;
                 //   FSLog.Info($"ReciprocalSystem，reciprocal：{ reciprocal.Value},timeSpan.TotalSeconds:{timeSpan.TotalSeconds}");

                }).Run();
        }
    }
}