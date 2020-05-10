using FootStone.ECS;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyCharPresentationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,
                ref Translation translation,
                ref Rotation rotation,
                in PhysicsVelocity physicsVelocity,
                in CharacterInterpolatedState interpolatedData) =>
            {
                translation.Value = interpolatedData.Position;
                rotation.Value = interpolatedData.Rotation;
            }).Run();
        }
    }


    [DisableAutoCreation]
    public class ApplyCharPredictedStateSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithNone<ServerEntity>().
                ForEach((Entity entity,
                ref TransformPredictedState transformState,
                in CharacterInterpolatedState interpolatedData) =>
            {
                transformState.Position = interpolatedData.Position;
                transformState.Rotation = interpolatedData.Rotation;
            }).Run();
        }
    }
}