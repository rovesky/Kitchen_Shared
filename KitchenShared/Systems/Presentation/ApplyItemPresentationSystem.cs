using FootStone.ECS;
using Unity.Entities;
using Unity.Physics;
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
                ref Rotation rotation
                // ref PhysicsVelocity physicsVelocity
            ) =>
            {
                translation.Value = interpolatedData.Position;
                rotation.Value = interpolatedData.Rotation;

                if (EntityManager.HasComponent<PhysicsVelocity>(entity))
                {
                    var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entity);
                    physicsVelocity.Linear = interpolatedData.Velocity;
                    EntityManager.SetComponentData(entity, physicsVelocity);
                }
                //   FSLog.Info($"physicsVelocity.Linear:{physicsVelocity.Linear}");

                if (interpolatedData.Owner != Entity.Null)
                {
                    if (!EntityManager.HasComponent<Parent>(entity))
                    {
                        EntityManager.AddComponentData(entity, new Parent());
                        EntityManager.AddComponentData(entity, new LocalToParent());
                    }

                    var parent = EntityManager.GetComponentData<Parent>(entity);
                  //  FSLog.Info($" parent.Value:{ parent.Value},entity:{entity},translation.Value:{translation.Value}");
                    if (parent.Value == interpolatedData.Owner)
                        return;
                    parent.Value = interpolatedData.Owner;

                    var scale = EntityManager.GetComponentData<Scale>(entity);
                    scale.Value = 1.0f;
                  //  FSLog.Info($"scale:{scale.Value}");
                    EntityManager.SetComponentData(entity, scale);
                    EntityManager.SetComponentData(entity, parent);
                }
                else
                {
                    if (!EntityManager.HasComponent<Parent>(entity))
                        return;
                    EntityManager.RemoveComponent<Parent>(entity);
                    EntityManager.RemoveComponent<LocalToParent>(entity);
                }
            });
        }
    }
}