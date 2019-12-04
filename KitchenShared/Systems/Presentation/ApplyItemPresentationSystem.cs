using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyItemPresentationSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity, 
                ref ItemInterpolatedState interpolatedData,
                ref Translation translation,
                ref Rotation rotation) =>
            {
            
                translation.Value = interpolatedData.Position;
                rotation.Value = interpolatedData.Rotation;

                if (interpolatedData.Owner != Entity.Null)
                {
                    if (!EntityManager.HasComponent<Parent>(entity))
                    {
                        EntityManager.AddComponentData(entity, new Parent());
                        EntityManager.AddComponentData(entity, new LocalToParent());
                    }

                    var parent = EntityManager.GetComponentData<Parent>(entity);
                    parent.Value = interpolatedData.Owner;
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
             //   FSLog.Info($"ApplyItemPresentationSystem,x:{interpolatedData.Position.x},z:{interpolatedData.Position.z}");

            });
        }
    }
}