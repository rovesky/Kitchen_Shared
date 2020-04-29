using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 米饭放入锅里
    /// </summary>
    [DisableAutoCreation]
    public class CharacterPutDownPotSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterPutDownPot")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in TriggerPredictedState triggerState,
                    in SlotPredictedState slotState,
                    in UserCommand command) =>
                {
                    //未按键返回
                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;

                    //没有拾取返回
                    var pickupEntity = slotState.FilledIn;
                    if (pickupEntity == Entity.Null)
                        return;

                    //拾取的道具不是unCooked返回
                    if (!EntityManager.HasComponent<Uncooked>(pickupEntity))
                        return;

                    //没有触发返回
                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    //触发的不是Table返回
                    if (!EntityManager.HasComponent<Table>(triggerEntity))
                        return;

                    //Table上没有道具返回
                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);
                    if (slot.FilledIn == Entity.Null)
                        return;

                    //Table上不是锅返回
                    if (!EntityManager.HasComponent<Pot>(slot.FilledIn))
                        return;

                    var potEntity = slot.FilledIn;
                    var potSlotState = EntityManager.GetComponentData<SlotPredictedState>(potEntity);

                    //锅已满
                    if (potSlotState.FilledIn != Entity.Null)
                        return;

                    //放入锅里
                    ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                        pickupEntity, potEntity, entity);

                 

                }).Run();
        }
    }

}