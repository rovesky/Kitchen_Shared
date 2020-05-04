using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 燃气灶着火
    /// </summary>
    [DisableAutoCreation]
    public class CatchFireSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity,TableCook>()
                .WithStructuralChanges()
                .WithNone<CatchFire>()
                .ForEach((Entity entity,
                    ref CatchFirePredictedState catchFireState,
                    in SlotPredictedState slotState ) =>
                {
                    if(catchFireState.IsCatchFire)
                        return;

                    if (slotState.FilledIn == Entity.Null)
                        return;

                    if (!EntityManager.HasComponent<Pot>(slotState.FilledIn))
                        return;

                    if (!EntityManager.HasComponent<SlotPredictedState>(slotState.FilledIn))
                        return;

                    var potSlot = EntityManager.GetComponentData<SlotPredictedState>(slotState.FilledIn);
                    if (potSlot.FilledIn == Entity.Null)
                        return;

                    //未煮熟返回
                    if(EntityManager.HasComponent<Uncooked>(potSlot.FilledIn))
                        return;

                    if(!EntityManager.HasComponent<FireAlertSetting>(slotState.FilledIn))
                        return;

                    var cookedSetting = EntityManager.GetComponentData<FireAlertSetting>(slotState.FilledIn);
                    var cookedState = EntityManager.GetComponentData<FireAlertPredictedState>(slotState.FilledIn);

                    FSLog.Info($"cookedState.CurFireAlertTick :{cookedState.CurTick}");
                    if (cookedState.CurTick < cookedSetting.TotalTick)
                    {
                        cookedState.CurTick ++;
                        EntityManager.SetComponentData(slotState.FilledIn, cookedState);
                        return;
                    }

                    catchFireState.IsCatchFire = true;
                    EntityManager.AddComponentData(entity, new CatchFire());

                }).Run();
        }
    }
}