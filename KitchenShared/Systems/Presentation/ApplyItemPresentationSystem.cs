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

               //  FSLog.Info($"interpolatedData.Position:{translation.Value},entity:{entity}");
               // if (Vector3.SqrMagnitude(physicsVelocity.Linear) < 1.0f)
                {
                    translation.Value = interpolatedData.Position;
                    rotation.Value = interpolatedData.Rotation;
                    //   FSLog.Info($"interpolatedData.Velocity:{interpolatedData.Velocity}");

                 //   physicsVelocity.Linear = interpolatedData.Velocity;
                }

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

                // var tick = GetSingleton<WorldTime>().Tick;

                //if (replicatedData.Id ==20)
                //    FSLog.Info($"ApplyItemPresentationSystem,tick:{tick},owner:{interpolatedData.Owner}" +
                //             $",pos:{translation.Value},HasParent:{EntityManager.HasComponent<Parent>(entity)}" +
                //             $",localToWorld:{localToWorld.Position},replicatedData.netId:{replicatedData.Id}");

            });
        }
    }
}