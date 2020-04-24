using FootStone.ECS;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 从洗碗池处拾取
    /// </summary>
    [DisableAutoCreation]
    public class CharacterPickupSinkSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterPickupSink")
                .WithStructuralChanges()
                .ForEach((Entity characterEntity,
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

                    //触发的不是洗碗池返回
                    if (!EntityManager.HasComponent<SinkSetting>(triggerEntity))
                        return;
        
                    //slot为空返回
                    var slot = EntityManager.GetComponentData<MultiSlotPredictedState>(triggerEntity);
                    if(slot.Value.IsEmpty())
                        return;

                    //拾取盘子
                    ItemAttachUtilities.ItemAttachToOwner(EntityManager, 
                        slot.Value.GetTail(), characterEntity,triggerEntity);
                    
                }).Run();
        }
      
    }
}