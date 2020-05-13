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

                    //已煮糊返回
                    if(EntityManager.HasComponent<Burnt>(potEntity))
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

                    fireAlertState.CurTick = 0;
                    EntityManager.SetComponentData(potEntity, fireAlertState);
                    
                    //燃气灶着火
                    catchFireState.IsCatchFire = true;
                    catchFireState.CurCatchFireTick = 0;
                    catchFireState.CurExtinguishTick = 0;
              
                    //锅烧糊
                    var potState = EntityManager.GetComponentData<PotPredictedState>(potEntity);
                    potState.State = PotState.Burnt;
                    EntityManager.SetComponentData(potEntity,potState);
            
                }).Run();
        }
    }
}