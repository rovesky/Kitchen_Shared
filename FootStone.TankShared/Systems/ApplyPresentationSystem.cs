using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [DisableAutoCreation]
    public class ApplyPresentationSystem : FSComponentSystem
    {
 

        protected override void OnCreate()
        {          
           
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref EntityPredictData predictData,
                ref Translation translation,ref Rotation rotation) =>
            {
                translation.Value = predictData.position;
                rotation.Value = predictData.rotation;
            });

        }
    }
}
