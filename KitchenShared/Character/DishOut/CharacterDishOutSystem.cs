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
            //食物在桌上
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterDishOut")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in TriggerPredictedState triggerState,
                    in SlotPredictedState slotState,
                    in TransformPredictedState transformState,
                    in UserCommand command) =>
                {
                    //未按键返回
                    if (!command.Buttons.IsSet(UserCommand.Button.Button1))
                        return;

                    //没有拾取返回
                    var pickupEntity = slotState.FilledIn;
                    if (pickupEntity == Entity.Null)
                        return;

                    //拾取的道具不是盘子返回
                    if (!HasComponent<Plate>(pickupEntity))
                        return;

                    //没有触发返回
                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    //触发的不是Table返回
                    if (!HasComponent<Table>(triggerEntity))
                        return;

                    //Table上道具不能装盘返回
                    var slot = GetComponent<SlotPredictedState>(triggerEntity);
                    if (slot.FilledIn == Entity.Null)
                        return;

                    //Table上道具不能装盘
                    if (!HasComponent<CanDishOut>(slot.FilledIn))
                        return;

                    var plateEntity = pickupEntity;
                    var foodEntity = slot.FilledIn;
                    var preOwner = triggerEntity;
                    
                    if(!DishOut(plateEntity,foodEntity,preOwner,transformState.Rotation))
                        // ReSharper disable once RedundantJumpStatement
                        return;

                }).Run();

              //食物在手上
              Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterDishOut1")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in TriggerPredictedState triggerState,
                    in SlotPredictedState slotState,
                    in TransformPredictedState transformState,
                    in UserCommand command) =>
                {
                    //未按键返回
                    if (!command.Buttons.IsSet(UserCommand.Button.Button1))
                        return;

                    //没有拾取返回
                    var pickupEntity = slotState.FilledIn;
                    if (pickupEntity == Entity.Null)
                        return;

                    var foodEntity = pickupEntity;
                    var preOwner = entity;
                    //拾取的道具是锅，并且锅没有糊，获取锅里的食物
                    if (HasComponent<Pot>(pickupEntity) && 
                        !HasComponent<Burnt>(pickupEntity))
                    {
                        preOwner = pickupEntity;
                        foodEntity = GetComponent<SlotPredictedState>(pickupEntity).FilledIn;
                    }


                    //拾取的道具是盘子，获取盘子里的最后一个食物
                    if (HasComponent<Plate>(pickupEntity))
                    {
                        preOwner = pickupEntity;
                        var multiSlotState = GetComponent<MultiSlotPredictedState>(pickupEntity);
                        foodEntity = multiSlotState.Value.GetTail();
                    }

                    if (foodEntity == Entity.Null)
                         return;
                   
                    //食物不能装盘返回
                    if (!HasComponent<CanDishOut>(foodEntity))
                        return;

                    //没有触发返回
                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    //触发的不是Table返回
                    if (!HasComponent<Table>(triggerEntity))
                        return;

                    //Table上没有道具返回
                    var slot = GetComponent<SlotPredictedState>(triggerEntity);
                    if (slot.FilledIn == Entity.Null)
                        return;

                    //Table上不是盘子返回
                    if (!HasComponent<Plate>(slot.FilledIn))
                        return;
                
                    if(!DishOut(slot.FilledIn,foodEntity,preOwner,transformState.Rotation))
                        return;

                    //锅设置为空
                    if (HasComponent<Pot>(preOwner))
                    {
                        var potState = GetComponent<PotPredictedState>(preOwner);
                        potState.State = PotState.Empty;
                        SetComponent(preOwner, potState);
                    }
                  
                }).Run();
        }

        public bool DishOut(/*UserCommand command,*/Entity plateEntity, 
            Entity foodEntity, Entity preOwner,quaternion rotation)
        {
            var plateSlotState = GetComponent<MultiSlotPredictedState>(plateEntity);

            //盘子已满
            if (plateSlotState.Value.IsFull())
                return false;

            //食材重复
            if (plateSlotState.Value.IsDuplicate(EntityManager, foodEntity))
                return false;

            //已经生成product
            var plateState = GetComponent<PlatePredictedState>(plateEntity);
            if (plateState.IsGenProduct)
                return false;
      
            //放入盘子
            ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                foodEntity, plateEntity, preOwner,float3.zero,rotation );

            //未成品，直接返回
            plateSlotState = GetComponent<MultiSlotPredictedState>(plateEntity);
            var menuTemplate = MenuUtilities.MatchMenuTemplate(EntityManager, plateSlotState);
            if (menuTemplate == MenuTemplate.Null)
                return true;

           // var worldTick = GetSingleton<WorldTime>().Tick;
           // FSLog.Info($"CharacterDishOut,command tick:{command.RenderTick},worldTick:{worldTick}");

            //删除原来的道具
            var count = plateSlotState.Value.Count();
            for (var i = 0; i < count; ++i)
            {
                var fillIn = plateSlotState.Value.TakeOut();
                if (fillIn == Entity.Null)
                    continue;

                var despawnState = GetComponent<DespawnPredictedState>(fillIn);
                despawnState.IsDespawn = true;
                despawnState.Tick = 0;
                SetComponent(fillIn, despawnState);
                //FSLog.Info($"despwan entity:{fillIn}");
            }
           
            SetComponent(plateEntity, plateSlotState);

            //生成新道具
            var spawnFoodEntity = GetSingletonEntity<SpawnItemArray>();
            var buffer = EntityManager.GetBuffer<SpawnItemRequest>(spawnFoodEntity);

          //  var spawnFoodArray = GetSingleton<SpawnItemArray>();

            buffer.Add(new SpawnItemRequest()
            {
                Type = menuTemplate.Product,
                DeferFrame = 0,
                OffPos = float3.zero,
                Owner = plateEntity,
                StartTick = GetSingleton<WorldTime>().Tick
            });

            plateState.IsGenProduct = true;
            SetComponent(plateEntity, plateState);
            return true;
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

                    if (!HasComponent<Product>(slotState.Value.FilledIn1))
                        return;

                    if(plateState.Product != Entity.Null)
                        return;

                    if(!plateState.IsGenProduct)
                        return;

                    plateState.Product = slotState.Value.FilledIn1;
                   // FSLog.Info($"UpdatePlateProductSystem:{plateState.Product}");

                }).Run();
        }
    }
}