using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class ApplyCharPresentationSystem : ComponentSystem
    {
 

        protected override void OnCreate()
        {          
           
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref CharacterInterpolateState predictData,
                ref Translation translation,ref Rotation rotation) =>
            {
                translation.Value = predictData.Position;
                translation.Value.y = 1.0f;
                rotation.Value = predictData.Rotation;
            });

        }
    }
}
