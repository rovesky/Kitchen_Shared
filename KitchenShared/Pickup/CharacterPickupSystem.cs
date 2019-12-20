using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupSystem : ComponentSystem
    {
      

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<ServerEntity>().ForEach((Entity entity,
                ref PickupSetting setting,
                ref UserCommand command,
                ref PickupPredictedState pickupState,
                ref TriggerPredictedState triggerState) =>
            {
                if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                    return;

                var triggerEntity = triggerState.TriggeredEntity;
                if (triggerEntity == Entity.Null)
                    return;

                var triggerData = EntityManager.GetComponentData<TriggerData>(triggerEntity);
                if ((triggerData.Type & (int) TriggerType.Table) == 0)
                    return;

                var worldTick = GetSingleton<WorldTime>().Tick;
                var isEmpty = pickupState.PickupedEntity == Entity.Null;
                var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);
                FSLog.Info($"TriggerOperationSystem Update,PickupedEntity:{pickupState.PickupedEntity}," +
                           $"triggerEntity:{triggerEntity}，slot.FiltInEntity:{slot.FilledInEntity}");

                if (isEmpty && slot.FilledInEntity != Entity.Null)
                {
                    FSLog.Info($"PickUpItem,command tick:{command.RenderTick},worldTick:{worldTick}");
                    PickUpItem(entity, triggerEntity, ref pickupState);
                }
                else if (!isEmpty && slot.FilledInEntity == Entity.Null)
                {
                    FSLog.Info($"PutDownItem,tick:{command.RenderTick},worldTick:{worldTick}");
                    PutDownItem(triggerEntity, ref pickupState);
                }
            });
        }

        private void PutDownItem(Entity overlapping,ref PickupPredictedState characterState)
        {
            var entity = characterState.PickupedEntity;
  
            var triggerData = EntityManager.GetComponentData<TriggerData>(overlapping);

            var itemPredictedState = EntityManager.GetComponentData<ItemPredictedState>(entity);
            itemPredictedState.Owner = Entity.Null;
            EntityManager.SetComponentData(entity, itemPredictedState);

            var itemEntityPredictedState = EntityManager.GetComponentData<EntityPredictedState>(entity);
            itemEntityPredictedState.Transform.pos = triggerData.SlotPos;
            itemEntityPredictedState.Transform.rot = quaternion.identity;
            itemEntityPredictedState.Velocity.Linear = float3.zero;
            EntityManager.SetComponentData(entity, itemEntityPredictedState);

            var replicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(entity);
            replicatedEntityData.PredictingPlayerId = -1;
            EntityManager.SetComponentData(entity, replicatedEntityData);

            //EntityManager.RemoveComponent<PhysicsVelocity>(entity);

            characterState.PickupedEntity = Entity.Null;

            var slot = EntityManager.GetComponentData<SlotPredictedState>(overlapping);
            slot.FilledInEntity = entity;
            EntityManager.SetComponentData(overlapping, slot);
        }

        private void PickUpItem(Entity owner, Entity triggerEntity, ref PickupPredictedState characterState)
        {
            var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);

            var entity = slot.FilledInEntity;

            var ownerReplicatedEntityData = EntityManager.GetComponentData<ReplicatedEntityData>(owner);
            EntityManager.AddComponentData(entity, new PickUpRequest()
            {
                PredictingPlayerId = ownerReplicatedEntityData.PredictingPlayerId,
                Owner =  owner
            });
          
            characterState.PickupedEntity = entity;
            slot.FilledInEntity = Entity.Null;

            EntityManager.SetComponentData(triggerEntity, slot);
            FSLog.Info($"triggerEntity:{triggerEntity}, slot: {slot.FilledInEntity}");
        }
    }
}