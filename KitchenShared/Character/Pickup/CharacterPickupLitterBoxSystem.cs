using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupLitterBoxSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterPickupLitterBox")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref SlotPredictedState slotState,
                    in TriggerPredictedState triggerState,
                    in UserCommand command) =>
                {
                    //未按键返回
                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;
                    
                    var pickupEntity = slotState.FilledIn;
                    //未拾取物品返回
                    if(pickupEntity == Entity.Null)
                        return;

                    //没有触发返回
                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    //触发的不是PlateRecycle返回
                    if (!EntityManager.HasComponent<LitterBox>(triggerEntity))
                        return;

                    if(!EntityManager.HasComponent<Food>(pickupEntity))
                        return;

                    EntityManager.AddComponentData(pickupEntity, new Despawn());
                    slotState.FilledIn = Entity.Null;


                }).Run();
        }
      
    }
}