using FootStone.ECS;
using Unity.Entities;


namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class FoodSlicedSystem : SystemBase
    {

        private EntityType FoodToSlice(EntityType foodType)
        {
            switch (foodType)
            {
                case EntityType.Shrimp:
                    return EntityType.ShrimpSlice;
                case EntityType.Cucumber:
                    return EntityType.CucumberSlice;
                default:
                    return EntityType.ShrimpSlice;
            }
        }

        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref ItemPredictedState itemState,
                    in FoodSlicedRequest request,
                    in Food food) =>
                {
                    EntityManager.RemoveComponent<FoodSlicedRequest>(entity);
                    EntityManager.AddComponentData(entity, new Despawn());
                
                    EntityManager.SetComponentData(itemState.Owner,new SlotPredictedState()
                    {
                        FilledInEntity = Entity.Null
                    });

                    var slotSetting = EntityManager.GetComponentData<SlotSetting>(itemState.Owner);

                    var spawnFoodEntity = GetSingletonEntity<SpawnFoodArray>();
                    var buffer = EntityManager.GetBuffer<SpawnFoodRequest>(spawnFoodEntity);
                    buffer.Add(new SpawnFoodRequest()
                    {
                        Type = FoodToSlice(food.Type),
                        Pos = slotSetting.Pos,
                        Owner = itemState.Owner,
                        IsSlice = true
                    });

                }).Run();
        }
    }
}


   
