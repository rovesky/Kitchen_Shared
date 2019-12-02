using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateItemPresentationSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<ServerEntity>()
                .ForEach((Entity entity, 
                    ref ItemPredictedState predictData,
                    ref ItemInterpolatedState interpolateData) =>
                {
                    interpolateData.Position = predictData.Position;
                    interpolateData.Rotation = predictData.Rotation;
                    interpolateData.Owner = predictData.Owner;
                    //     FSLog.Info($"UpdateCharPresentationSystem,x:{predictData.Position.x},z:{predictData.Position.z}");
                });
        }
    }
}