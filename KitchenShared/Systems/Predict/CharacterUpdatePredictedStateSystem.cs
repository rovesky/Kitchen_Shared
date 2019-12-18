using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterUpdatePredictedStateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,
                ref CharacterPredictedState predictedState,
                ref Translation translation,
                ref Rotation rotation,
                ref PhysicsVelocity physicsVelocity) =>
            {
                translation.Value = predictedState.Position;
                rotation.Value = predictedState.Rotation;
                physicsVelocity.Linear = predictedState.LinearVelocity;
            });
        }
    }
}