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
                    ref EntityPredictedState entityPredictData,
                    ref ItemPredictedState predictData,
                    ref ItemInterpolatedState interpolateData) =>
                {
                    interpolateData.Position = entityPredictData.Transform.pos;
                    interpolateData.Rotation = entityPredictData.Transform.rot;
                    interpolateData.Velocity = entityPredictData.Velocity.Linear;
                    interpolateData.Owner = predictData.Owner;

                  //  FSLog.Info($"interpolateData.Velocity :{interpolateData.Velocity}");
                    //   FSLog.Info($"UpdateItemPresentationSystem,Position:{predictData.Position}");
                });
        }
    }
}