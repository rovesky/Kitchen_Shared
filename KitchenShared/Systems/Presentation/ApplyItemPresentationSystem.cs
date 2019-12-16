using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

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
                ref Rotation rotation,
                ref PhysicsVelocity physicsVelocity,
                //    ref LocalToWorld localToWorld,
                ref ReplicatedEntityData replicatedData
            ) =>
            {

                translation.Value = interpolatedData.Position;
                rotation.Value = interpolatedData.Rotation;
                physicsVelocity.Linear = interpolatedData.Velocity;
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