using FootStone.ECS;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;

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

                    //触发的不是PlateRecycle返回
                    if (!EntityManager.HasComponent<PlateRecycle>(triggerEntity))
                        return;
        
                    //slot为空返回
                    var slot = EntityManager.GetComponentData<MultiSlotPredictedState>(triggerEntity);
                    if(slot.Value.IsEmpty())
                        return;


                    //将盘子叠起来
                    var count = slot.Value.Count();

                    var firstEntity = slot.Value.GetTail();
                    var prePlateEntity = Entity.Null;
                    for (var i = 0; i < count; ++i)
                    {
                        slot = EntityManager.GetComponentData<MultiSlotPredictedState>(triggerEntity);
                        var plateEntity = slot.Value.GetTail();

                        if (prePlateEntity == Entity.Null)
                            ItemAttachUtilities.ItemDetachFromOwner(EntityManager, plateEntity,
                                triggerEntity, float3.zero, float3.zero);
                        else
                            ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                                plateEntity, prePlateEntity, triggerEntity);

                        prePlateEntity = plateEntity;
                    }

                    ItemAttachUtilities.ItemAttachToOwner(EntityManager, 
                        firstEntity, characterEntity,triggerEntity);

                }).Run();
        }
      
    }
}