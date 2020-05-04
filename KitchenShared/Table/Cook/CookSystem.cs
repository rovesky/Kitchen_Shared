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

                    if (!EntityManager.HasComponent<Pot>(slotState.FilledIn))
                        return;

                    if (!EntityManager.HasComponent<SlotPredictedState>(slotState.FilledIn))
                        return;

                    var potSlot = EntityManager.GetComponentData<SlotPredictedState>(slotState.FilledIn);
                    if (potSlot.FilledIn == Entity.Null)
                        return;

                    if(EntityManager.HasComponent<Cooked>(potSlot.FilledIn))
                        return;

                    if(!EntityManager.HasComponent<CookedSetting>(slotState.FilledIn))
                        return;

                    var cookedSetting = EntityManager.GetComponentData<FoodSlicedSetting>(slotState.FilledIn);
                    var cookedState = EntityManager.GetComponentData<FoodSlicedState>(slotState.FilledIn);

                  //  FSLog.Info($"cookedState.CurSliceTick :{cookedState.CurSliceTick}");
                    if (cookedState.CurSliceTick < cookedSetting.TotalSliceTick)
                    {
                        cookedState.CurSliceTick ++;
                        EntityManager.SetComponentData(slotState.FilledIn, cookedState);
                        return;
                    }


                    if (!HasSingleton<SpawnItemArray>())
                        return;
                    //删除生米饭
                    //ItemAttachUtilities.ItemDetachFromOwner(EntityManager,
                    //    potSlot.FilledIn,slotState.FilledIn,
                    //    float3.zero, quaternion.identity,float3.zero);
                    EntityManager.AddComponentData(potSlot.FilledIn, new Despawn()
                    {
                        Frame = 1
                    });

                           
                    //生成熟米饭
                    var spawnItemEntity = GetSingletonEntity<SpawnItemArray>();
                    var buffer = EntityManager.GetBuffer<SpawnItemRequest>(spawnItemEntity);
                    buffer.Add(new SpawnItemRequest()
                    {
                        Type = EntityType.RiceCooked,
                        Owner = slotState.FilledIn

                    });

                }).Run();
        }
    }
}