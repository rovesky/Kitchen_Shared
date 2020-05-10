using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 食物装盘
    /// </summary>
    [DisableAutoCreation]
    public class CharacterDishOutSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterDishOut")
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

                    var preOwner = entity;
                    //拾取的道具是锅，并且锅没有糊，获取锅里的道具
                    if (EntityManager.HasComponent<Pot>(pickupEntity) && 
                        !EntityManager.HasComponent<Burnt>(pickupEntity))
                    {
                        preOwner = pickupEntity;
                        pickupEntity = EntityManager.GetComponentData<SlotPredictedState>(pickupEntity).FilledIn;
                    }

                    if (pickupEntity == Entity.Null)
                         return;
                   
                    //拾取的道具不能装盘返回
                    if (!EntityManager.HasComponent<CanDishOut>(pickupEntity))
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

                    //Table上不是盘子返回
                    if (!EntityManager.HasComponent<Plate>(slot.FilledIn))
                        return;

                    var plateEntity = slot.FilledIn;
                    var plateSlotState = EntityManager.GetComponentData<MultiSlotPredictedState>(plateEntity);

                    //盘子已满
                    if (plateSlotState.Value.IsFull())
                        return;

                    //食材重复
                    if (plateSlotState.Value.IsDuplicate(EntityManager,pickupEntity))
                        return;

                    //放入盘子
                    ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                        pickupEntity, plateEntity, preOwner);

                    //未成品，直接返回
                    plateSlotState = EntityManager.GetComponentData<MultiSlotPredictedState>(plateEntity);
                    var menuTemplate = MenuUtilities.MatchMenuTemplate(EntityManager, plateSlotState);
                    if(menuTemplate == MenuTemplate.Null)
                        return;

                    
                    //删除原来的道具
                    var count = plateSlotState.Value.Count();
                    for (var i = 0; i < count; ++i)
                    {
                        var fillIn = plateSlotState.Value.TakeOut();
                        if (fillIn != Entity.Null)
                            EntityManager.AddComponentData(fillIn,new Despawn());
                    }
                    EntityManager.SetComponentData(plateEntity,plateSlotState);

            
                    //生成新道具
                    var spawnFoodEntity = GetSingletonEntity<SpawnItemArray>();
                    var buffer = EntityManager.GetBuffer<SpawnItemRequest>(spawnFoodEntity);
                    
                    buffer.Add(new SpawnItemRequest()
                    {
                        Type = menuTemplate.Product,
                        DeferFrame = 0,
                        OffPos = float3.zero,
                        Owner = plateEntity,
                        StartTick = GetSingleton<WorldTime>().Tick
                    });

                }).Run();
        }
    }


    [DisableAutoCreation]
    public class UpdatePlateProductSystem : SystemBase
    {
        
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((ref PlatePredictedState plateState,
                    in MultiSlotPredictedState slotState) =>
                {
                  
                    if(slotState.Value.Count() != 1)
                        return;

                    if (!EntityManager.HasComponent<Product>(slotState.Value.FilledIn1))
                        return;
                    plateState.Product = slotState.Value.FilledIn1;

                }).Run();
        }
    }
}