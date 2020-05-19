using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 扔垃圾
    /// </summary>
    [DisableAutoCreation]
    public class CharacterDropLitterSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterDropLitter")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref SlotPredictedState slotState,
                    in TriggerPredictedState triggerState,
                    in UserCommand command) =>
                {
                    //未按键返回
                    if (!command.Buttons.IsSet(UserCommand.Button.Button1))
                        return;

                    var pickupEntity = slotState.FilledIn;
                    //未拾取物品返回
                    if (pickupEntity == Entity.Null)
                        return;

                    //没有触发返回
                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    //触发的不是垃圾桶返回
                    if (!EntityManager.HasComponent<TableLitterBox>(triggerEntity))
                        return;

                    //不是食物返回
                    if (!EntityManager.HasComponent<Food>(pickupEntity))
                        return;

                    var despawnState = EntityManager.GetComponentData<DespawnPredictedState>(pickupEntity);
                    despawnState.IsDespawn = true;
                    despawnState.Tick = 0;
                    EntityManager.SetComponentData(pickupEntity,despawnState);

                    slotState.FilledIn = Entity.Null;

                }).Run();
        }

    }
}