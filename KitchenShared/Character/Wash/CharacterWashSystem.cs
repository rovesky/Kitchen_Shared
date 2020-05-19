using FootStone.ECS;
using Unity.Entities;

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
                    if (!command.Buttons.IsSet(UserCommand.Button.Button2))
                        return;

                    if (washState.IsWashing)
                        return;

                    var pickupedEntity = slotState.FilledIn;
                    if (pickupedEntity != Entity.Null)
                        return;

                    var triggeredEntity = triggerState.TriggeredEntity;
                    if (triggeredEntity == Entity.Null)
                        return;

                    if (!EntityManager.HasComponent<TableSink>(triggeredEntity))
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

                    //未触发或者触发的不是水槽，打断清洗操作
                    var triggeredEntity = triggerState.TriggeredEntity;
                    if (triggeredEntity == Entity.Null ||
                        !EntityManager.HasComponent<TableSink>(triggeredEntity))
                    {
                        washState.IsWashing = false;
                        return;
                    }

                    //水槽脏盘子已经洗完，打断清洗操作
                    var sink = EntityManager.GetComponentData<SinkPredictedState>(triggeredEntity);
                    if (sink.Value.IsEmpty())
                    {
                        washState.IsWashing = false;
                    }

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
                     //水槽没有脏盘子
                    var sinkState = EntityManager.GetComponentData<SinkPredictedState>(triggeredEntity);
                    if (sinkState.Value.IsEmpty())
                        return;

                    var plateDirty = sinkState.Value.GetTail();

                    //该脏盘子还未洗完
                    var washProgressState = EntityManager.GetComponentData<ProgressPredictState>(plateDirty);
                    var washProgressSetting = EntityManager.GetComponentData<ProgressSetting>(plateDirty);

                    if (washProgressState.CurTick < washProgressSetting.TotalTick)
                    {
                        washProgressState.CurTick++;
                        EntityManager.SetComponentData(plateDirty, washProgressState);
                        return;
                    }

                    //脏盘子已经洗完
                   
                    //从水槽中取出
                    sinkState.Value.TakeOut();
                    EntityManager.SetComponentData(triggeredEntity, sinkState);

                    if (HasSingleton<SpawnItemArray>())
                    {
                        //删除脏盘子
                        var plateDirtyDespawn = EntityManager.GetComponentData<DespawnPredictedState>(plateDirty);
                        plateDirtyDespawn.IsDespawn = true;
                        plateDirtyDespawn.Tick = 0;
                        EntityManager.SetComponentData(plateDirty,plateDirtyDespawn);


                        //生成干净盘子
                        var spawnItemEntity = GetSingletonEntity<SpawnItemArray>();
                        var buffer = EntityManager.GetBuffer<SpawnItemRequest>(spawnItemEntity);
                        buffer.Add(new SpawnItemRequest()
                        {
                            Type = EntityType.Plate,
                            Owner = triggeredEntity,
                            StartTick = GetSingleton<WorldTime>().Tick
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