using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;


namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class PlateServedSystem : SystemBase
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
            Entities.WithAll<ServerEntity,Plate>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref PlatePredictedState plateState,
                    in ItemPredictedState itemState,
                    in PlateServedRequest request) =>
                {
                    EntityManager.RemoveComponent<PlateServedRequest>(entity);

                    EntityManager.AddComponentData(entity, new Despawn());
                    EntityManager.SetComponentData(itemState.Owner,new SlotPredictedState()
                    {
                        FilledInEntity = Entity.Null
                    });


                    var query = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(Score)
                        }
                    });
                    var entities = query.ToEntityArray(Allocator.TempJob);
                    if(entities.Length  < 1)
                        return;

                    var scoreEntity = entities[0];
                    var score = EntityManager.GetComponentData<Score>(scoreEntity);
                    score.Value += 100;
                    EntityManager.SetComponentData(scoreEntity,score);
                    entities.Dispose();
                }).Run();
        }
    }
}


   
