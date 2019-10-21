using FootStone.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [DisableAutoCreation]
    public class MovePositionSystem : FSComponentSystem
    {
        protected override void OnUpdate()
        {
            //Entities.ForEach((ref MovePosition moveMouse,ref UserCommand playerCommand, ref Translation position) =>
            //{
            //    if (playerCommand.buttons.IsSet(UserCommand.Button.Move))
            //    {
            //        var tickDuration = GetSingleton<WorldTime>().tick.TickDuration;
            //        // 使用Vector3提供的MoveTowards函数，获得朝目标移动的位置
            //        Vector3 pos = Vector3.MoveTowards(position.Value, playerCommand.targetPos, moveMouse.Speed * tickDuration);
            //        // 更新当前位置
            //        position.Value = pos;
            //    }
            //});

            var tickDuration = GetSingleton<WorldTime>().TickDuration;
            Entities.ForEach((ref MovePosition moveMouse, ref UserCommand playerCommand, ref EntityPredictData predictData) =>
            {
                if (playerCommand.buttons.IsSet(UserCommand.Button.Move))
                {                 
                    // 使用Vector3提供的MoveTowards函数，获得朝目标移动的位置
                    Vector3 pos = Vector3.MoveTowards(predictData.position, playerCommand.targetPos, moveMouse.Speed * tickDuration);
                    // 更新当前位置
                    predictData.position = pos;
                }
            });
        }
    }
}
