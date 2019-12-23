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
                    ref TransformPredictedState transformPredictData,
                    ref VelocityPredictedState velocityPredictData,
                    ref ItemPredictedState predictData,
                    ref ItemInterpolatedState interpolateData) =>
                {
                    interpolateData.Position = transformPredictData.Position;
                    interpolateData.Rotation = transformPredictData.Rotation;
                    interpolateData.Velocity = velocityPredictData.Linear;
                    interpolateData.Owner = predictData.Owner;

                  //  FSLog.Info($"interpolateData.Velocity :{interpolateData.Velocity}");
                    //   FSLog.Info($"UpdateItemPresentationSystem,Position:{predictData.Position}");
                });
        }
    }
}