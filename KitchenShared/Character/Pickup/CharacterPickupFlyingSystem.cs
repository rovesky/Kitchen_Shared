using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupFlyingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                ref PickupPredictedState pickupState,
                ref TriggerPredictedState triggerState,
                in PickupSetting setting,
                in UserCommand command,
                in ReplicatedEntityData replicatedEntityData) =>
            {
                if (command.Buttons.IsSet(UserCommand.Button.Throw))
                   return;
               
                if (pickupState.PickupedEntity != Entity.Null || triggerState.TriggeredEntity == Entity.Null) 
                    return;
               
                var triggerData = EntityManager.GetComponentData<TriggerData>(triggerState.TriggeredEntity);
                if ((triggerData.Type & (int) TriggerType.Item) == 0)
                    return;
                   
                var item = EntityManager.GetComponentData<ItemPredictedState>(triggerState.TriggeredEntity);
                FSLog.Info($"PickUpItem item.TempOwner:{item.TempOwner},entity:{entity}");
                if(item.TempOwner == Entity.Null ||  item.TempOwner == entity)
                    return;

                //TODO 需要判断triggerState.TriggeredEntity的状态是否能发request

                var worldTick = GetSingleton<WorldTime>().Tick;
                FSLog.Info($"PickUpItem flying,command tick:{command.RenderTick},worldTick:{worldTick}");
                  
                EntityManager.AddComponentData(triggerState.TriggeredEntity, new ItemAttachToCharacterRequest
                {
                    PredictingPlayerId = replicatedEntityData.PredictingPlayerId,
                    Owner = entity
                });

                pickupState.PickupedEntity = triggerState.TriggeredEntity;
                triggerState.TriggeredEntity = Entity.Null;
            }).Run();
        }
    }
}