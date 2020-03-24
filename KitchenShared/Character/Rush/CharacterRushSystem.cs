using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterRushSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .ForEach((Entity entity,
                    ref CharacterMovePredictedState movePredictData,
                    in TransformPredictedState transformData,
                    in RushSetting setting,
                    in UserCommand command) =>
            {
              
                if (!command.Buttons.IsSet(UserCommand.Button.Rush))
                    return;
             
                Vector3 linear = math.mul(transformData.Rotation, Vector3.forward);
                linear.Normalize();
                linear *= setting.Velocity;
               // FSLog.Info($"CharacterRushSystem Update,linear：{linear}");

                movePredictData.ImpulseVelocity = linear;
                movePredictData.ImpulseDuration = setting.Duration;
                    
            }).Run();
        }
    }


   
}