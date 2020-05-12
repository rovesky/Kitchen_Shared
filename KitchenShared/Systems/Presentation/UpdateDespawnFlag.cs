using FootStone.ECS;
using Unity.Entities;


namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateDespawnFlag : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                in DespawnPredictedState despawnState) =>
            {

                if (despawnState.IsDespawn)
                    EntityManager.AddComponentData(entity, new Despawn()
                    {
                        Tick = despawnState.Tick
                    });
                else if (EntityManager.HasComponent<Despawn>(entity))
                    EntityManager.RemoveComponent<Despawn>(entity);

            }).Run();
        }
    }
}