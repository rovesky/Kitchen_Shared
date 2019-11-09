using FootStone.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [DisableAutoCreation]
    public class MoveInputSystem : FSComponentSystem
    {
        protected override void OnUpdate()
        {        

            var tickDuration = GetSingleton<WorldTime>().TickDuration;
            var tick = GetSingleton<WorldTime>().Tick;
            Entities.ForEach((ref MoveInput moveMouse, ref UserCommand playerCommand, ref EntityPredictData predictData) =>
            {
              //  FSLog.Info("MoveInputSystem OnUpdate!");
                float3 pos = predictData.position + (float3)playerCommand.targetPos * moveMouse.Speed * tickDuration;
                predictData.position = pos;
            });
        }
    }
}
