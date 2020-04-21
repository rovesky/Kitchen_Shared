using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ServeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity,TableServe>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in SlotPredictedState slotState) =>
                {
                    if(slotState.FilledIn == Entity.Null)
                        return;

                    FSLog.Info("TableServeSystem OnUpdate");
                    var filledInEntity = slotState.FilledIn;

                    if(!EntityManager.HasComponent<Plate>(filledInEntity))
                        return;

                    EntityManager.AddComponentData(filledInEntity,new PlateServedRequest());

                }).Run();
        }
    }
}


   
