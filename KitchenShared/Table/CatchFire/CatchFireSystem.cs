using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CatchFireSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in CatchFirePredictedState catchFireState) =>
                {
                    if (catchFireState.IsCatchFire)
                        EntityManager.AddComponentData(entity, new CatchFire());
                    else if (EntityManager.HasComponent<CatchFire>(entity))
                        EntityManager.RemoveComponent<CatchFire>(entity);

                }).Run();
        }
    }
}


   
