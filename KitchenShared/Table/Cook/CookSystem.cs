using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 煮饭
    /// </summary>
    [DisableAutoCreation]
    public class CookSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity, TableCook>()
                .WithNone<CatchFire>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in SlotPredictedState slotState) =>
                {
                    if (slotState.FilledIn == Entity.Null)
                        return;

                    //灶上不是锅返回
                    if (!EntityManager.HasComponent<Pot>(slotState.FilledIn))
                        return;

                    var potEntity = slotState.FilledIn;
                    //锅已煮糊返回
                    if(EntityManager.HasComponent<Burnt>(potEntity))
                        return;
                    
                    //锅里没有米饭返回
                    if (!EntityManager.HasComponent<SlotPredictedState>(potEntity))
                        return;

                    var potSlot = EntityManager.GetComponentData<SlotPredictedState>(potEntity);
                    if (potSlot.FilledIn == Entity.Null)
                        return;

                    var potState = EntityManager.GetComponentData<PotPredictedState>(potEntity);
                    if(potState.State != PotState.Full)
                        return;

                    if(!EntityManager.HasComponent<FireAlertPredictedState>(potEntity))
                        return;

                    var cookedSetting = EntityManager.GetComponentData<ProgressSetting>(potEntity);
                    var cookedState = EntityManager.GetComponentData<ProgressPredictState>(potEntity);

                  //  FSLog.Info($"cookedState.CurSliceTick :{cookedState.CurSliceTick}");
                    if (cookedState.CurTick < cookedSetting.TotalTick)
                    {
                        cookedState.CurTick ++;
                        EntityManager.SetComponentData(potEntity, cookedState);
                        return;
                    }

                    cookedState.CurTick = 0;
                    EntityManager.SetComponentData(potEntity, cookedState);

                    //锅已煮好
                  //  var potState = EntityManager.GetComponentData<PotPredictedState>(potEntity);
                    potState.State = PotState.Cooked;
                    EntityManager.SetComponentData(potEntity,potState);

                    if (!HasSingleton<SpawnItemArray>())
                        return;


                    
                    //删除生米饭
                    var riceDespawn = EntityManager.GetComponentData<DespawnPredictedState>(potSlot.FilledIn);
                    riceDespawn.IsDespawn = true;
                    riceDespawn.Tick = 1;
                    EntityManager.SetComponentData( potSlot.FilledIn,riceDespawn);

                    //锅置空
                    potSlot.FilledIn = Entity.Null;
                    EntityManager.SetComponentData(potEntity,potSlot);
                           
                    //生成熟米饭
                    var spawnItemEntity = GetSingletonEntity<SpawnItemArray>();
                    var buffer = EntityManager.GetBuffer<SpawnItemRequest>(spawnItemEntity);
                    buffer.Add(new SpawnItemRequest()
                    {
                        Type = EntityType.RiceCooked,
                        Owner = slotState.FilledIn,
                        StartTick = GetSingleton<WorldTime>().Tick

                    });

                }).Run();
        }
    }
}