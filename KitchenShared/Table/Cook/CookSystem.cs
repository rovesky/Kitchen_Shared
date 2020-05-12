using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

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

                    //米饭已经煮熟返回
                    if(EntityManager.HasComponent<Cooked>(potSlot.FilledIn))
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

                    if (!HasSingleton<SpawnItemArray>())
                        return;

                 
                    //锅置空
                    potSlot.FilledIn = Entity.Null;
                    EntityManager.SetComponentData(potEntity,potSlot);

                    //删除生米饭
                    var potDespawn = EntityManager.GetComponentData<DespawnPredictedState>(potEntity);
                    potDespawn.IsDespawn = true;
                    potDespawn.Tick = 1;
                    EntityManager.SetComponentData(potEntity,potDespawn);
                           
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