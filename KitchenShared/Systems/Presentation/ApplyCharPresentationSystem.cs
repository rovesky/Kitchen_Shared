using FootStone.ECS;
using Unity.Entities;
using Unity.Physics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyCharPresentationSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,
              //  ref CharacterPredictedState predictedState,
                ref CharacterInterpolatedState interpolatedData,
                ref Translation translation,
                ref Rotation rotation,
                ref PhysicsVelocity physicsVelocity) =>
            {
              //  predictedState.Position.y = 1.2f;
                translation.Value = interpolatedData.Position;
                rotation.Value = interpolatedData.Rotation;
              //  physicsVelocity.Linear = interpolatedData.LinearVelocity;
            });
        }
    }
}