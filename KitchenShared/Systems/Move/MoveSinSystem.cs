using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class MoveSinSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var tickDuration = GetSingleton<WorldTime>().TickDuration;
            var tick = GetSingleton<WorldTime>().Tick;
            Entities.WithAllReadOnly<MoveSin>().ForEach((ref CharacterPredictState predictData) =>
            {        
                // 左右移动
                float rx = Mathf.Sin(tick/30.0f) * tickDuration;

                predictData.Position = new float3(predictData.Position.x + rx,
                                       predictData.Position.y,
                                       predictData.Position.z);              

            });
        }
    }

}
