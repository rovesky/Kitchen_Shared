using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 烧糊,燃气灶着火
    /// </summary>
    [DisableAutoCreation]
    public class BurntSystem : SystemBase
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

                    var potEntity = slotState.FilledIn;
                    if (!EntityManager.HasComponent<SlotPredictedState>(potEntity))
                        return;

                  
                    var potSlot = EntityManager.GetComponentData<SlotPredictedState>(potEntity);
                    if (potSlot.FilledIn == Entity.Null)
                        return;

                    //未煮熟返回
                    if(EntityManager.HasComponent<Uncooked>(potSlot.FilledIn))
                        return;

                    //没有着火警告组件返回
                    if(!EntityManager.HasComponent<FireAlertSetting>(potEntity))
                        return;

                    var fireAlertSetting = EntityManager.GetComponentData<FireAlertSetting>(potEntity);
                    var fireAlertState = EntityManager.GetComponentData<FireAlertPredictedState>(potEntity);

                    FSLog.Info($"cookedState.CurFireAlertTick :{fireAlertState.CurTick}");
                    if (fireAlertState.CurTick < fireAlertSetting.TotalTick)
                    {
                        fireAlertState.CurTick ++;
                        EntityManager.SetComponentData(potEntity, fireAlertState);
                        return;
                    }

                    //燃气灶着火
                    catchFireState.IsCatchFire = true;
                 //   EntityManager.AddComponentData(entity, new CatchFire());

                    //锅烧糊
                    var burntState = EntityManager.GetComponentData<BurntPredictedState>(potEntity);
                    burntState.IsBurnt = true;
                    EntityManager.SetComponentData(potEntity,burntState);
                    EntityManager.AddComponentData(potEntity, new Burnt());

                }).Run();
        }
    }
}