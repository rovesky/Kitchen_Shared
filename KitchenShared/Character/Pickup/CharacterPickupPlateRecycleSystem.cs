using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 从盘子回收处拾取
    /// </summary>
    [DisableAutoCreation]
    public class CharacterPickupPlateRecycleSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterPickupTable")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in TriggerPredictedState triggerState,
                    in SlotPredictedState slotState,
                    in UserCommand command) =>
                {
                    //未按键返回
                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;
                    
                    //已拾取物品返回
                    var pickupEntity = slotState.FilledIn;
                    if(pickupEntity != Entity.Null)
                        return;

                    //没有触发返回
                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    //触发的不是PlateRecycle返回
                    if (!EntityManager.HasComponent<PlateRecycle>(triggerEntity))
                        return;
        
                    //slot为空返回
                    var slot = EntityManager.GetComponentData<MultiSlotPredictedState>(triggerEntity);
                    if(slot.IsEmpty())
                        return;
                    
                    ItemAttachUtilities.ItemAttachToOwner(EntityManager, 
                        slot.GetTail(), entity,triggerEntity);

                }).Run();
        }
      
    }
}