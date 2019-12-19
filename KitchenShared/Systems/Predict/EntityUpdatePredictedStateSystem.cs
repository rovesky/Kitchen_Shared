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
                ref EntityPredictedState predictedState,
                ref Translation translation,
                ref Rotation rotation,
                ref PhysicsVelocity physicsVelocity) =>
            {
                translation.Value = predictedState.Transform.pos;
                rotation.Value = predictedState.Transform.rot;
                physicsVelocity = predictedState.Velocity;
            });
        }
    }
}