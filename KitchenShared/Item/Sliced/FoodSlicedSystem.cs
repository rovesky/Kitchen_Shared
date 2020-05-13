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
                    ref DespawnPredictedState despawnState,
                    in  OwnerPredictedState ownerState,
                    in FoodSlicedRequest request,
                    in GameEntity food) =>
                {
                    EntityManager.RemoveComponent<FoodSlicedRequest>(entity);
                  
                    //删除原来的食物
                    despawnState.IsDespawn = true;
                    despawnState.Tick = 0;
                   
                    EntityManager.SetComponentData(ownerState.Owner,new SlotPredictedState()
                    {
                        FilledIn = Entity.Null
                    });


                    //生成切好的食物
                    var slotSetting = EntityManager.GetComponentData<SlotSetting>(ownerState.Owner);

                    var spawnFoodEntity = GetSingletonEntity<SpawnItemArray>();
                    var buffer = EntityManager.GetBuffer<SpawnItemRequest>(spawnFoodEntity);
                    buffer.Add(new SpawnItemRequest()
                    {
                        Type = FoodToSlice(food.Type),
                        OffPos = slotSetting.Pos,
                        Owner = ownerState.Owner,
                        StartTick = GetSingleton<WorldTime>().Tick
                    });

                }).Run();
        }
    }
}


   
