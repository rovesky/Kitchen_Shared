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
            Entities.WithAllReadOnly<Player>().ForEach((Entity entity,ref PickupItem pickupItem,ref UserCommand command,ref CharacterPredictState predictData) =>
            {
                //  FSLog.Info("PickSystem Update");
                if (command.buttons.IsSet(UserCommand.Button.Throw) )
                {
                    if (predictData.PickupedEntity == Entity.Null)
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
							predictData.PickupedEntity = e;
							if (!EntityManager.HasComponent<Parent>(predictData.PickupedEntity))
							{
								EntityManager.AddComponentData(predictData.PickupedEntity, new Parent() { Value = entity });
								EntityManager.AddComponentData(predictData.PickupedEntity, new LocalToParent());
								EntityManager.SetComponentData(predictData.PickupedEntity, new Translation() { Value = new float3(0, 0.2f, 0.8f) });
								EntityManager.SetComponentData(predictData.PickupedEntity, new Rotation() { Value = quaternion.identity });

								var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(predictData.PickupedEntity);
								physicsVelocity.Linear = float3.zero;
								EntityManager.SetComponentData(predictData.PickupedEntity, physicsVelocity);

							}
							plate.IsFree = false;
							EntityManager.SetComponentData(e, plate);
							break;
						}
						plates.Dispose();
                    }
                    else
                    {
                        EntityManager.RemoveComponent<Parent>(predictData.PickupedEntity);
                        EntityManager.RemoveComponent<LocalToParent>(predictData.PickupedEntity);          
                     
                        var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(predictData.PickupedEntity);
                        Vector3 linear = math.mul(predictData.Rotation, Vector3.forward);
                        linear.y = 0.4f;
                        linear.Normalize();
                        physicsVelocity.Linear = linear * 18;
                        EntityManager.SetComponentData(predictData.PickupedEntity,physicsVelocity);

                        EntityManager.SetComponentData(predictData.PickupedEntity, new Translation()
                          { Value = predictData.Position + math.mul(predictData.Rotation, new float3(0, 0.2f, 0.8f))  });

						//   var pickupItemData = EntityManager.GetComponentData<EntityPredictData>(pickupItem.pickupEntity);
						//   pickupItemData.Position = predictData.Position + math.mul(predictData.rotation, new float3(0, 0.2f, 1.2f));
						////   pickupItemData.rotation = quaternion.identity;
						//   EntityManager.SetComponentData(pickupItem.pickupEntity, pickupItemData);

						var plate = EntityManager.GetComponentData<Plate>(predictData.PickupedEntity);
						plate.IsFree = true;
						EntityManager.SetComponentData(predictData.PickupedEntity, plate);
						predictData.PickupedEntity = Entity.Null;
                    }                   
                }
            });
        }      
    }
}