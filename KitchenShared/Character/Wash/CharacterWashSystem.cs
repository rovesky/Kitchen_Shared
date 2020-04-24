using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 洗碗开始
    /// </summary>
    [DisableAutoCreation]
    public class CharacterWashStartSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref WashPredictedState washState,
                    in SlotPredictedState slotState,
                    in TriggerPredictedState triggerState,
                    in UserCommand command) =>
                {
                    if (!command.Buttons.IsSet(UserCommand.Button.Throw))
                        return;

                    if (washState.IsWashing)
                        return;

                    var pickupedEntity = slotState.FilledIn;
                    if (pickupedEntity != Entity.Null)
                        return;

                    var triggeredEntity = triggerState.TriggeredEntity;
                    if (triggeredEntity == Entity.Null)
                        return;

                    if (!EntityManager.HasComponent<SinkSetting>(triggeredEntity))
                        return;

                    var sink = EntityManager.GetComponentData<SinkPredictedState>(triggeredEntity);
                    if (sink.Value.IsEmpty())
                        return;

                    FSLog.Info($"Character Wash Begin!");
                    washState.IsWashing = true;
                }).Run();
        }
    }

    /// <summary>
    /// 洗碗中断
    /// </summary>
    [DisableAutoCreation]
    public class CharacterWashInterruptSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref WashPredictedState washState,
                    in SlotPredictedState slotState,
                    in TriggerPredictedState triggerState,
                    in UserCommand command) =>
                {

                    if (!washState.IsWashing)
                        return;

                    var pickupedEntity = slotState.FilledIn;
                    if (pickupedEntity != Entity.Null)
                    {
                        washState.IsWashing = false;
                        return;
                    }

                    var triggeredEntity = triggerState.TriggeredEntity;
                    if (triggeredEntity == Entity.Null)

                        if (!EntityManager.HasComponent<SinkSetting>(triggeredEntity))
                        {
                            washState.IsWashing = false;
                            return;
                        }

                    var sink = EntityManager.GetComponentData<SinkPredictedState>(triggeredEntity);
                    if (sink.Value.IsEmpty())
                    {
                        washState.IsWashing = false;
                        return;
                    }

                   // FSLog.Info($"Character Wash Interrupt!");

                }).Run();
        }
    }

    /// <summary>
    /// 洗碗过程
    /// </summary>
    [DisableAutoCreation]
    public class CharacterWashingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref WashPredictedState washState,
                    in SlotPredictedState slotState,
                    in TriggerPredictedState triggerState) =>
                {
                    if (!washState.IsWashing)
                        return;


                    var triggeredEntity = triggerState.TriggeredEntity;
                    var sinkSetting = EntityManager.GetComponentData<SinkSetting>(triggeredEntity);
                    var sinkState = EntityManager.GetComponentData<SinkPredictedState>(triggeredEntity);
                    if (sinkState.Value.IsEmpty())
                        return;

                    var multiSlotState = EntityManager.GetComponentData<MultiSlotPredictedState>(triggeredEntity);


                    var plateDirty = sinkState.Value.GetTail();

                    var itemSliceState = EntityManager.GetComponentData<FoodSlicedState>(plateDirty);
                    var itemSliceSetting = EntityManager.GetComponentData<FoodSlicedSetting>(plateDirty);

                    if (itemSliceState.CurSliceTick < itemSliceSetting.TotalSliceTick)
                    {
                        itemSliceState.CurSliceTick++;
                        EntityManager.SetComponentData(plateDirty, itemSliceState);
                        return;
                    }

                    //水槽中取出
                    sinkState.Value.TakeOut();
                    EntityManager.SetComponentData(triggeredEntity,sinkState);
                    
                 
                    if (HasSingleton<SpawnItemArray>())
                    {
                        //删除脏盘子
                        EntityManager.AddComponentData(plateDirty, new Despawn()
                        {
                            Frame = 1
                        });

                           
                        //生成干净盘子
                        var spawnItemEntity = GetSingletonEntity<SpawnItemArray>();
                        var buffer = EntityManager.GetBuffer<SpawnItemRequest>(spawnItemEntity);
                        buffer.Add(new SpawnItemRequest()
                        {
                            Type = EntityType.Plate,
                            Owner = triggeredEntity

                        });
                    }

                }).Run();
        }
    }


    [DisableAutoCreation]
    public class CharacterWashSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterWashStartSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterWashInterruptSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterWashingSystem>());
     
        }
    }
}