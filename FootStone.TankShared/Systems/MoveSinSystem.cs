using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class MoveSinSystem : FSComponentSystem
    {
        protected override void OnUpdate()
        {
            var tickDuration = GetSingleton<WorldTime>().TickDuration;
            var tick = GetSingleton<WorldTime>().Tick;
            Entities.WithAllReadOnly<MoveSin>().ForEach((ref EntityPredictData predictData) =>
            {        
                // 左右移动
                float rx = Mathf.Sin(tick/30.0f) * tickDuration;

                predictData.position = new float3(predictData.position.x + rx,
                                       predictData.position.y,
                                       predictData.position.z);              

            });
        }
    }

}
