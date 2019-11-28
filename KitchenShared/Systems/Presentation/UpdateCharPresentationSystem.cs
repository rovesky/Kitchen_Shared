using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class UpdateCharPresentationSystem : ComponentSystem
    {
 

        protected override void OnCreate()
        {          
           
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref CharacterPredictedState predictData,
                ref CharacterInterpolatedState interpolateData) =>
            {
                interpolateData.Position = predictData.Position;
                interpolateData.Rotation = predictData.Rotation;
            });

        }
    }
}
