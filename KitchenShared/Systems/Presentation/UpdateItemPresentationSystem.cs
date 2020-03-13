using FootStone.ECS;
using Unity.Entities;
using Unity.Physics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateItemPresentationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref ItemInterpolatedState interpolateData,
                    in TransformPredictedState transformPredictData,
                    in VelocityPredictedState velocityPredictData,
                    in ItemPredictedState predictData) =>
                {
                    interpolateData.Position = transformPredictData.Position;
                    interpolateData.Rotation = transformPredictData.Rotation;
                    interpolateData.Owner = predictData.Owner;

                    switch (velocityPredictData.MotionType)
                    {
                        case MotionType.Dynamic:
                            EntityManager.AddComponentData(entity, new PhysicsVelocity()
                            {
                                Linear = velocityPredictData.Linear,
                                Angular = velocityPredictData.Angular
                            });
                            break;
                        case MotionType.Static:
                            EntityManager.RemoveComponent<PhysicsVelocity>(entity);
                            break;
                    }
                    // FSLog.Info($"UpdateItemPresentationSystem,Position:{interpolateData.Position}");
                }).Run();
        }
    }
}