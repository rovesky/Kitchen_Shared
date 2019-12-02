using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateCharPresentationSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<ServerEntity>()
                .ForEach((Entity entity, ref CharacterPredictedState predictData,
                    ref CharacterInterpolatedState interpolateData) =>
                {
                    interpolateData.Position = predictData.Position;
                    interpolateData.Rotation = predictData.Rotation;
                    //     FSLog.Info($"UpdateCharPresentationSystem,x:{predictData.Position.x},z:{predictData.Position.z}");
                });
        }
    }
}