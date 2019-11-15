using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [DisableAutoCreation]
    public class PickupSystem : ComponentSystem
    {
        private EntityQuery plateQuery;

        protected override void OnCreate()
        {
            base.OnCreate();
            plateQuery = GetEntityQuery(typeof(Plate));
        }

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<Player>().ForEach((Entity entity,ref PickupItem pickupItem,ref UserCommand command,ref EntityPredictData predictData) =>
            {
                if (!command.buttons.IsSet(UserCommand.Button.Pick))
                    return;

                if (EntityManager.HasComponent<ReleaseItem>(entity))
                    return;

                var pickupEntity = predictData.pickupEntity;
                if (pickupEntity != Entity.Null)
                    return;

                if (plateQuery.CalculateEntityCount() == 0)
                    return;

                var plates = plateQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
                pickupEntity = plates[0];
                plates.Dispose();

                if (EntityManager.HasComponent<Parent>(pickupEntity))
                    return;

                FSLog.Info($"Pick Command:{command.checkTick},{command.renderTick},{pickupEntity}");
                predictData.pickupEntity = pickupEntity;


                //   FSLog.Info("Pickup item");
                EntityManager.AddComponentData(pickupEntity, new Parent() { Value = entity });
                EntityManager.AddComponentData(pickupEntity, new LocalToParent());

                //var pickupEntityData = EntityManager.GetComponentData<EntityPredictData>(pickupEntity);
                //pickupEntityData.position = new float3(0, 0.2f, 0.8f);
                //pickupEntityData.rotation = quaternion.identity;
                //EntityManager.SetComponentData(pickupEntity, pickupEntityData);
                EntityManager.SetComponentData(pickupEntity, new Translation() { Value = new float3(0, 0.2f, 0.8f) });
                EntityManager.SetComponentData(pickupEntity, new Rotation() { Value = quaternion.identity });

                var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(pickupEntity);
                physicsVelocity.Linear = Vector3.zero;
                EntityManager.SetComponentData(pickupEntity, physicsVelocity);

              

            });
        }      
    }
}