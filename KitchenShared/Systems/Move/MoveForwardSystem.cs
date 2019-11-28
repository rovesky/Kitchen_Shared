using FootStone.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class MoveForwardSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            var tickDuration = GetSingleton<WorldTime>().GameTick.TickDuration;

            Entities.ForEach((ref CharacterPredictedState predictData, ref MoveForward move) =>
            {                  
               
                float3 forward = (Quaternion)predictData.Rotation * Vector3.forward;
             //   FSLog.Info($"MoveForward,forward：[{forward.x},{forward.y},{forward.z}]");                
                predictData.Position = predictData.Position - forward * move.Speed * tickDuration;
               
            });

        }
    }
}
