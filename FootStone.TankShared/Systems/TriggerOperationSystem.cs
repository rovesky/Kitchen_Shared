using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace Assets.Scripts.ECS
{
	[DisableAutoCreation]
	public class TriggerOperationSystem : ComponentSystem
	{
		EntityQuery m_OverlappingGroup;
		protected override void OnCreate()
		{
			m_OverlappingGroup = GetEntityQuery(typeof(OverlappingTriggerComponent));
		}

		protected override void OnUpdate()
		{
			//FSLog.Info("TriggerOperationSystem Update");
			Entities.WithAllReadOnly<Player>().ForEach((Entity entity, ref PickupItem pickupItem, ref UserCommand command, ref EntityPredictData predictData) =>
			{
				if (command.buttons.IsSet(UserCommand.Button.Pick))
				{
					var isEmpty = predictData.pickupEntity == Entity.Null;
					var entities = m_OverlappingGroup.ToEntityArray(Allocator.TempJob);
					
					for (int i = 0; i < entities.Length; ++i)
					{
						var overlapping = entities[i];
						FSLog.Info(overlapping);
						//var overlappingData = EntityManager.GetComponentData<OverlappingTriggerComponent>(overlapping);
						//if (overlappingData.TriggerIndex != entity.Index)
						//{
						//	continue;
						//}
						var triggerData = EntityManager.GetComponentData<TriggerDataComponent>(overlapping);
						if ((triggerData.VolumeType & (int)TriggerVolumeType.Table) != 0)
						{
							var slot = EntityManager.GetComponentData<SlotComponent>(overlapping);
							if (isEmpty && slot.FiltInEntity != Entity.Null)
							{
								PickUpItem(entity, overlapping, ref predictData, ref slot);
							}
							else if(!isEmpty && slot.FiltInEntity == Entity.Null)
							{
								PutDownItem(entity, overlapping, ref predictData, ref slot);
							}
						}
						break;
					}

					entities.Dispose();
				}
			});
		}

		private void PutDownItem(Entity owner, Entity overlapping, ref EntityPredictData item, ref SlotComponent slot)
		{
			FSLog.Info("PutDownItem");
			var entity = item.pickupEntity;
			EntityManager.RemoveComponent<Parent>(entity);
			EntityManager.RemoveComponent<LocalToParent>(entity);

			var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entity);
			physicsVelocity.Linear = float3.zero;

			var pos = EntityManager.GetComponentData<LocalToWorld>(slot.SlotEntity);
			EntityManager.SetComponentData(entity, new Translation() { Value = pos.Position });
			EntityManager.SetComponentData(entity, new Rotation() { Value = quaternion.identity });
			EntityManager.SetComponentData(entity, physicsVelocity);

			item.pickupEntity = Entity.Null;
			slot.FiltInEntity = entity;

			EntityManager.SetComponentData(overlapping, slot);
		}

		private void PickUpItem(Entity owner, Entity overlapping, ref EntityPredictData item, ref SlotComponent slot)
		{
			FSLog.Info("PickUpItem");
			var entity = slot.FiltInEntity;

			EntityManager.AddComponentData(entity, new Parent() { Value = owner });
			EntityManager.AddComponentData(entity, new LocalToParent());
			EntityManager.SetComponentData(entity, new Translation() { Value = new float3(0, 0.2f, 0.8f) });
			EntityManager.SetComponentData(entity, new Rotation() { Value = quaternion.identity });

			var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entity);
			physicsVelocity.Linear = float3.zero;

			EntityManager.SetComponentData(entity, physicsVelocity);

			item.pickupEntity = entity;
			slot.FiltInEntity = Entity.Null;

			EntityManager.SetComponentData(overlapping, slot);
		}
	}
}