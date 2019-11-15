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
                //  FSLog.Info("PickSystem Update");
                if (command.buttons.IsSet(UserCommand.Button.Throw) )
                {
                    if (predictData.pickupEntity == Entity.Null)
                    {
                        if (plateQuery.CalculateEntityCount() == 0)
                            return;

                        var plates = plateQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);
						for (int i = 0; i < plates.Length; ++i)
						{
							var e = plates[i];
							var plate = EntityManager.GetComponentData<Plate>(e);
							if (!plate.IsFree)
							{
								continue;
							}
							predictData.pickupEntity = e;
							if (!EntityManager.HasComponent<Parent>(predictData.pickupEntity))
							{
								EntityManager.AddComponentData(predictData.pickupEntity, new Parent() { Value = entity });
								EntityManager.AddComponentData(predictData.pickupEntity, new LocalToParent());
								EntityManager.SetComponentData(predictData.pickupEntity, new Translation() { Value = new float3(0, 0.2f, 0.8f) });
								EntityManager.SetComponentData(predictData.pickupEntity, new Rotation() { Value = quaternion.identity });

								var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(predictData.pickupEntity);
								physicsVelocity.Linear = float3.zero;
								EntityManager.SetComponentData(predictData.pickupEntity, physicsVelocity);

							}
							plate.IsFree = false;
							EntityManager.SetComponentData(e, plate);
							break;
						}
						plates.Dispose();
                    }
                    else
                    {
                        EntityManager.RemoveComponent<Parent>(predictData.pickupEntity);
                        EntityManager.RemoveComponent<LocalToParent>(predictData.pickupEntity);          
                     
                        var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(predictData.pickupEntity);
                        Vector3 linear = math.mul(predictData.rotation, Vector3.forward);
                        linear.y = 0.4f;
                        linear.Normalize();
                        physicsVelocity.Linear = linear * 10;
                        EntityManager.SetComponentData(predictData.pickupEntity,physicsVelocity);

                        EntityManager.SetComponentData(predictData.pickupEntity, new Translation()
                          { Value = predictData.position + math.mul(predictData.rotation, new float3(0, 0.2f, 1.2f))  });

						//   var pickupItemData = EntityManager.GetComponentData<EntityPredictData>(pickupItem.pickupEntity);
						//   pickupItemData.position = predictData.position + math.mul(predictData.rotation, new float3(0, 0.2f, 1.2f));
						////   pickupItemData.rotation = quaternion.identity;
						//   EntityManager.SetComponentData(pickupItem.pickupEntity, pickupItemData);

						var plate = EntityManager.GetComponentData<Plate>(predictData.pickupEntity);
						plate.IsFree = true;
						EntityManager.SetComponentData(predictData.pickupEntity, plate);
						predictData.pickupEntity = Entity.Null;
                    }                   
                }
            });
        }      
    }
}