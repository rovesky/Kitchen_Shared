using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;

namespace FootStone.Kitchen
{
	[DisableAutoCreation]
	public class CharacterPickupSystem : ComponentSystem
	{
		EntityQuery m_OverlappingGroup;
		protected override void OnCreate()
		{
			m_OverlappingGroup = GetEntityQuery(typeof(OverlappingTriggerComponent));
		}

		protected override void OnUpdate()
		{
			//FSLog.Info("TriggerOperationSystem Update");
			Entities.WithAllReadOnly<Character>().ForEach((Entity entity, 
                ref CharacterPickupItem pickupItem,
                ref UserCommand command,
                ref CharacterPredictedState predictData) =>
            {
                if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                    return;
          
                var isEmpty = predictData.PickupedEntity == Entity.Null;
                var entities = m_OverlappingGroup.ToEntityArray(Allocator.TempJob);
              
                foreach (var overlappingEntity in  entities)
                {
                 //   FSLog.Info(overlappingEntity);
                    var overlappingData = EntityManager.GetComponentData<OverlappingTriggerComponent>(overlappingEntity);
                    if (overlappingData.TriggerEntity != entity.Index)
                        continue;
                  
                    var worldTick = GetSingleton<WorldTime>().Tick;
                    var triggerData = EntityManager.GetComponentData<TriggerDataComponent>(overlappingEntity);

                     if ((triggerData.VolumeType & (int) TriggerVolumeType.Table) != 0)
                    {
                        var slot = EntityManager.GetComponentData<SlotComponent>(overlappingEntity);
                        FSLog.Info($"TriggerOperationSystem Update,isEmpty:{isEmpty},slot.FiltInEntity:{slot.FiltInEntity}");

                        if (isEmpty && slot.FiltInEntity != Entity.Null)
                        {
                            FSLog.Info($"PickUpItem,command tick:{command.CheckTick},worldTick:{worldTick}");
                            PickUpItem(entity, overlappingEntity, ref predictData, ref slot);
                        }
                        else if (!isEmpty && slot.FiltInEntity == Entity.Null)
                        {
                            FSLog.Info($"PutDownItem,tick:{command.CheckTick},worldTick:{worldTick}");
                            PutDownItem(entity, overlappingEntity, ref predictData, ref slot);
                        }
                    }

                    break;
                }

                entities.Dispose();

            });
		}

		private void PutDownItem(Entity owner, Entity overlapping, ref CharacterPredictedState characterState, ref SlotComponent slot)
		{
			var entity = characterState.PickupedEntity;
		
			var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entity);
			physicsVelocity.Linear = float3.zero;
            EntityManager.SetComponentData(entity, physicsVelocity);

            var pos = EntityManager.GetComponentData<LocalToWorld>(slot.SlotEntity);
            var itemPredictedState = EntityManager.GetComponentData<ItemPredictedState>(entity);
            itemPredictedState.Position = pos.Position;
            itemPredictedState.Rotation = quaternion.identity;
            itemPredictedState.Owner = Entity.Null;
            EntityManager.SetComponentData(entity, itemPredictedState);

            var replicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(entity);
            replicatedEntityData.PredictingPlayerId = -1;
            EntityManager.SetComponentData(entity, replicatedEntityData);

        
            characterState.PickupedEntity = Entity.Null;

			slot.FiltInEntity = entity;
			EntityManager.SetComponentData(overlapping, slot);
		}

        private void PickUpItem(Entity owner, Entity overlapping, ref CharacterPredictedState characterState,
            ref SlotComponent slot)
        {
            var entity = slot.FiltInEntity;
          //  EntityManager.AddComponentData(entity, new Parent() {Value = owner});
          //  EntityManager.AddComponentData(entity, new LocalToParent());
            //	EntityManager.SetComponentData(entity, new Translation() { Value = new float3(0, 0.2f, 0.8f) });
            //	EntityManager.SetComponentData(entity, new Rotation() { Value = quaternion.identity });

            var physicsVelocity = EntityManager.GetComponentData<PhysicsVelocity>(entity);
            physicsVelocity.Linear = float3.zero;
            EntityManager.SetComponentData(entity, physicsVelocity);

            var itemPredictedState = EntityManager.GetComponentData<ItemPredictedState>(entity);
            itemPredictedState.Position = new float3(0, 0.2f, 0.8f);
            itemPredictedState.Rotation = quaternion.identity;
            itemPredictedState.Owner = owner;
            EntityManager.SetComponentData(entity, itemPredictedState);


            var ownerReplicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(owner);
            var replicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(entity);
            replicatedEntityData.PredictingPlayerId = ownerReplicatedEntityData.PredictingPlayerId;
            EntityManager.SetComponentData(entity, replicatedEntityData);

            characterState.PickupedEntity = entity;

            slot.FiltInEntity = Entity.Null;
            EntityManager.SetComponentData(overlapping, slot);
        }
    }
}