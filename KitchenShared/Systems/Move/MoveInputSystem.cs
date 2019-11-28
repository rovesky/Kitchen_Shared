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

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class MoveInputSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {        

            var tickDuration = GetSingleton<WorldTime>().TickDuration;
            var tick = GetSingleton<WorldTime>().Tick;
            Entities.ForEach((ref MoveInput moveMouse, ref UserCommand playerCommand, ref CharacterPredictedState predictData) =>
            {
              //  FSLog.Info("MoveInputSystem OnUpdate!");
                float3 pos = predictData.Position + (float3)playerCommand.targetPos * moveMouse.Speed * tickDuration;
                predictData.Position = pos;
            });
        }
    }
}
