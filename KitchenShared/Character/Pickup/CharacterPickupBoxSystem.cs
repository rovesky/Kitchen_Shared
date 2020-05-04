using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 从箱子拾取食品
    /// </summary>
    [DisableAutoCreation]
    public class CharacterPickupBoxSystem : SystemBase 
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterPickupBoxSystem")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in SlotPredictedState slotState,
                    in TriggerPredictedState triggerState,
                    in UserCommand command) =>
                {

                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;

                    var pickupedEntity = slotState.FilledIn;
                    if (pickupedEntity != Entity.Null)
                        return;

                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    if(!EntityManager.HasComponent<TableBox>(triggerEntity))
                        return;

                    if(EntityManager.HasComponent<CatchFire>(triggerEntity))
                        return;

                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);
                    if(slot.FilledIn != Entity.Null)
                        return;

                  //  FSLog.Info("pick up box!");

                    var slotSetting = EntityManager.GetComponentData<SlotSetting>(triggerEntity);
                    var boxSetting = EntityManager.GetComponentData<TableBox>(triggerEntity);

                    var spawnFoodEntity = GetSingletonEntity<SpawnItemArray>();
                    var buffer = EntityManager.GetBuffer<SpawnItemRequest>(spawnFoodEntity);
                    
                    buffer.Add(new SpawnItemRequest()
                    {
                        Type = boxSetting.Type,
                        OffPos = slotSetting.Pos,
                        DeferFrame = 15,
                        Owner = entity
                    });

                     EntityManager.AddComponentData(triggerEntity,new BoxOpenRequest());

                }).Run();
        }

    }
}