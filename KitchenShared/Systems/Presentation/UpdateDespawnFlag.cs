using FootStone.ECS;
using Unity.Entities;


namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateDespawnState : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                ref DespawnPredictedState despawnState) =>
            {
                if (EntityManager.HasComponent<Despawn>(entity))
                    EntityManager.RemoveComponent<Despawn>(entity);
               
                if (!despawnState.IsDespawn) 
                    return;

                if (despawnState.Tick == 0)
                    EntityManager.AddComponentData(entity, new Despawn());
                else
                    despawnState.Tick--;

            }).Run();
        }
    }
}