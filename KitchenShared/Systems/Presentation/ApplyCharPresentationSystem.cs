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
}