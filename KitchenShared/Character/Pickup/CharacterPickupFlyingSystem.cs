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
                ref TriggerPredictedState triggerState,
                in SlotPredictedState slotState,
                in UserCommand command) =>
              {

                //  FSLog.Info($"CharacterPickupFlyingSystem,entity:{entity}");
                if (command.Buttons.IsSet(UserCommand.Button.Throw))
                   return;

                var pickupEntity = slotState.FilledIn;
               
                if (pickupEntity != Entity.Null || triggerState.TriggeredEntity == Entity.Null) 
                    return;;

                if(!EntityManager.HasComponent<Item>(triggerState.TriggeredEntity))
                    return;
                   
                var item = EntityManager.GetComponentData<OwnerPredictedState>(triggerState.TriggeredEntity);
              //  FSLog.Info($"PickUpItem flying,PreOwner:{item.PreOwner},entity:{entity}");
                if(item.PreOwner == Entity.Null ||  item.PreOwner == entity)
                    return;

                var worldTick = GetSingleton<WorldTime>().Tick;
                FSLog.Info($"PickUpItem flying,command tick:{command.RenderTick},worldTick:{worldTick}");
 
                ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                    triggerState.TriggeredEntity,entity,Entity.Null);
                  
            //    pickupState.PickupedEntity = triggerState.TriggeredEntity;
                triggerState.TriggeredEntity = Entity.Null;

            }).Run();
        }
    }
}