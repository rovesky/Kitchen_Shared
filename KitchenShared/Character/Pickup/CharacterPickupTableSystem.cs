using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupTableSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<ServerEntity>().ForEach((Entity entity,
                ref PickupSetting setting,
                ref UserCommand command,
                ref PickupPredictedState pickupState,
                ref TriggerPredictedState triggerState,
                ref ReplicatedEntityData replicatedEntityData) =>
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
                 
                    EntityManager.AddComponentData(triggerEntity, new DetachFromTableRequest());
              
                    EntityManager.AddComponentData(slot.FilledInEntity, new AttachToCharacterRequest()
                    {
                        PredictingPlayerId = replicatedEntityData.PredictingPlayerId,
                        Owner = entity
                    });

                    pickupState.PickupedEntity = slot.FilledInEntity;
                }
                else if (!isEmpty && slot.FilledInEntity == Entity.Null)
                {
                    FSLog.Info($"PutDownItem,tick:{command.RenderTick},worldTick:{worldTick}");

                    EntityManager.AddComponentData(pickupState.PickupedEntity, new DetachFromCharacterRequest()
                    {
                        Pos = float3.zero,
                        LinearVelocity = float3.zero
                    });

                    var request = new AttachToTableRequest()
                    {
                        ItemEntity = pickupState.PickupedEntity,
                        SlotPos = triggerData.SlotPos
                    };
                    EntityManager.AddComponentData(triggerEntity, request);
                    EntityManager.AddComponentData(pickupState.PickupedEntity, request);

                    pickupState.PickupedEntity = Entity.Null;
                }
            });
        }
      
    }
}