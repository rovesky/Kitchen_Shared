using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class ApplyCharPresentationSystem : ComponentSystem
    {

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,ref CharacterInterpolatedState predictData,
                ref Translation translation,ref Rotation rotation) =>
            {
                translation.Value = predictData.Position;
                translation.Value.y = 1.0f;
                rotation.Value = predictData.Rotation;
            });

        }
    }
}
