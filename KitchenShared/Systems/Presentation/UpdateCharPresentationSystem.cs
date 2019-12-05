using FootStone.ECS;
using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateCharPresentationSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<ServerEntity>()
                .ForEach((Entity entity, 
                    ref CharacterPredictedState predictData,
                    ref CharacterInterpolatedState interpolateData,
                    ref UserCommand command) =>
                {
                    interpolateData.Position = predictData.Position;
                    interpolateData.Rotation = predictData.Rotation;
                    interpolateData.SqrMagnitude = new Vector2(command.TargetDir.x, command.TargetDir.z).sqrMagnitude;
                    //     FSLog.Info($"UpdateCharPresentationSystem,x:{predictData.Position.x},z:{predictData.Position.z}");
                });
        }
    }
}