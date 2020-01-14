using FootStone.ECS;
using Unity.Entities;
using Unity.Physics;

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
                 //   interpolateData.Velocity = velocityPredictData.Linear;
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
                });
        }
    }
}