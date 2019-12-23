using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class EntityUpdatePredictedStateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,
                ref TransformPredictedState transformPredictedState,
                ref VelocityPredictedState velocityPredictedState,
                ref Translation translation,
                ref Rotation rotation) =>
            {
                translation.Value = transformPredictedState.Position;
                rotation.Value = transformPredictedState.Rotation;
                if (EntityManager.HasComponent<PhysicsVelocity>(entity))
                {
                    EntityManager.SetComponentData(entity, new PhysicsVelocity()
                    {
                        Linear = velocityPredictedState.Linear,
                        Angular = velocityPredictedState.Angular
                    });
                }
            });
        }
    }
}

