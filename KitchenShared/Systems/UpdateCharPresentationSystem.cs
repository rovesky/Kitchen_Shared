using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class UpdateCharPresentationSystem : ComponentSystem
    {
 

        protected override void OnCreate()
        {          
           
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref CharacterPredictState predictData,
                ref CharacterInterpolateState interpolateData) =>
            {
                interpolateData.position = predictData.position;
                interpolateData.rotation = predictData.rotation;
            });

        }
    }
}
