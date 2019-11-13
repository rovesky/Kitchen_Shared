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

            var tickDuration = GetSingleton<WorldTime>().TickDuration;
            var tick = GetSingleton<WorldTime>().Tick;
            Entities.ForEach((ref MovePosition moveMouse, ref UserCommand playerCommand, ref EntityPredictData predictData) =>
            {
                if (playerCommand.buttons.IsSet(UserCommand.Button.Pick))
                {
                //    FSLog.Info($"command MovePosition:{playerCommand.renderTick},{playerCommand.checkTick},{tick} ");
                    // 使用Vector3提供的MoveTowards函数，获得朝目标移动的位置
                    Vector3 pos = Vector3.MoveTowards(predictData.position, playerCommand.targetPos, moveMouse.Speed * tickDuration);
                    // 更新当前位置
                    predictData.position = pos;
                }
            });
        }
    }
}
