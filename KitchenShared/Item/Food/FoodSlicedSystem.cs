using FootStone.ECS;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;


namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class FoodSlicedSystem : SystemBase
    {
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
                        Type = EntityType.AppleSlice,
                        Pos = slotSetting.Pos,
                        Owner = itemState.Owner,
                        IsSlice = true
                    });

                }).Run();
        }
    }
}


   
