using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [DisableAutoCreation]
    public class ItemStateServerSystem : ComponentSystem
    {
        
        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Plate>().ForEach((Entity entity,ref ItemInterpolatedState state,
                ref Translation translation, ref Rotation rotation) =>
            {
                state.position = translation.Value;
                state.rotation = rotation.Value;

                if (EntityManager.HasComponent<Parent>(entity))
                {
                    state.owner = EntityManager.GetComponentData<Parent>(entity).Value;
                }
                else
                {
                    state.owner = Entity.Null;
                }
      
            });
        }      
    }
}