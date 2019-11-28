using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class ApplyItemPresentationSystem :ComponentSystem
    {
 

        protected override void OnCreate()
        {          
           
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, ref ItemInterpolatedState predictData,
                ref Translation translation,ref Rotation rotation) =>
            {
                translation.Value = predictData.position;
                rotation.Value = predictData.rotation;

                if (predictData.owner != Entity.Null)
                {
                    if (!EntityManager.HasComponent<Parent>(entity))
                    {
                        EntityManager.AddComponentData(entity, new Parent());
                        EntityManager.AddComponentData(entity, new LocalToParent());
                    }

                    var parent = EntityManager.GetComponentData<Parent>(entity);
                    parent.Value = predictData.owner;
                    EntityManager.SetComponentData(entity, parent);
                }
                else
                {
                    if (EntityManager.HasComponent<Parent>(entity))
                    {
                        EntityManager.RemoveComponent<Parent>(entity);
                        EntityManager.RemoveComponent<LocalToParent>(entity);                       
                    }
                }
            });

        }
    }
}
