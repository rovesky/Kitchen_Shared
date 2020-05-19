using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterRushStartSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .ForEach((Entity entity,
                    ref CharacterMovePredictedState movePredictData,
                    ref RushPredictState rushState,
                    in TransformPredictedState transformState,
                    in RushSetting setting,
                    in UserCommand command) =>
                {

                    if (!command.Buttons.IsSet(UserCommand.Button.Rush))
                        return;

                    if (rushState.CurCooldownTick > 0)
                        return;

                    Vector3 linear = math.mul(transformState.Rotation, Vector3.forward);
                    linear.Normalize();
                    linear *= setting.Velocity;
                    // FSLog.Info($"CharacterRushSystem Update,linear：{linear}");

                    movePredictData.ImpulseVelocity = linear;
                    movePredictData.ImpulseDuration = setting.DurationTick;

                    rushState.IsRushed = true;
                    rushState.CurCooldownTick = setting.CooldownTick;
                    FSLog.Info($"CharacterRushSystem, rushState.CurCooldownTick:{rushState.CurCooldownTick} ");

                }).Run();
        }
    }

    [DisableAutoCreation]
    public class CharacterRushEndSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .ForEach((Entity entity,
                    ref RushPredictState rushState) =>
                {
                    if (!rushState.IsRushed)
                        return;

                    if (rushState.CurCooldownTick > 0)
                    {
                        rushState.CurCooldownTick--;
                        return;
                    }
                    rushState.IsRushed = false;

                 //   FSLog.Info($"CharacterRushCooldownSystem, rushState.CurCooldownTick:{rushState.CurCooldownTick} ");

                }).Run();
        }
    }
}