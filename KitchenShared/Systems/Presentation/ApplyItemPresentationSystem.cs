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
                ref Rotation rotation
                // ref PhysicsVelocity physicsVelocity
            ) =>
            {
                translation.Value = interpolatedData.Position;
                rotation.Value = interpolatedData.Rotation;

                //if (EntityManager.HasComponent<PhysicsVelocity>(entity))
                //{
                //    var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entity);
                //    physicsVelocity.Linear = interpolatedData.Velocity;
                //    EntityManager.SetComponentData(entity, physicsVelocity);
                //}
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
                    //var scale = EntityManager.GetComponentData<Scale>(entity);
                    //scale.Value = 1.0f;
                  //  FSLog.Info($"scale:{scale.Value}");
                   // EntityManager.SetComponentData(entity, scale);
                    EntityManager.SetComponentData(entity, parent);


                    var scale = EntityManager.GetComponentData<CompositeScale>(entity);
                    var parentScale = EntityManager.GetComponentData<CompositeScale>(interpolatedData.Owner);
                    scale.Value.c0.x /= parentScale.Value.c0.x;
                    scale.Value.c1.y /= parentScale.Value.c1.y;
                    scale.Value.c2.z /= parentScale.Value.c2.z;
                    EntityManager.SetComponentData(entity, scale);
                }
                else
                {
                    if (!EntityManager.HasComponent<Parent>(entity))
                        return;

                    var parent =  EntityManager.GetComponentData<Parent>(entity);
                    var parentScale = EntityManager.GetComponentData<CompositeScale>(parent.Value);
                    var scale = EntityManager.GetComponentData<CompositeScale>(entity);
                    scale.Value.c0.x *= parentScale.Value.c0.x;
                    scale.Value.c1.y *= parentScale.Value.c1.y;
                    scale.Value.c2.z *= parentScale.Value.c2.z;
                    EntityManager.SetComponentData(entity, scale);

                    EntityManager.RemoveComponent<Parent>(entity);
                    EntityManager.RemoveComponent<LocalToParent>(entity);
                }
            });
        }
    }
}