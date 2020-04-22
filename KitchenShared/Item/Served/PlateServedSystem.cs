using System.Linq;
using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class PlateServedSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity, Plate>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in PlatePredictedState plateState,
                    in OwnerPredictedState itemState,
                    in PlateServedRequest request) =>
                {

                    FSLog.Info("PlateServedSystem OnUpdate");

                    //add score
                    ushort score = CalculateScore(plateState.Product);
                    AddScore(score);

                    EntityManager.RemoveComponent<PlateServedRequest>(entity);
                    
                    //Despawn
                    Despawn(entity);

                    EntityManager.SetComponentData(itemState.Owner, new SlotPredictedState()
                    {
                        FilledIn = Entity.Null
                    });

                }).Run();
        }

        private void Despawn(Entity entity)
        {
            var slotState = EntityManager.GetComponentData<MultiSlotPredictedState>(entity);
            var count = slotState.Count();
        //    FSLog.Info($"Count:{count}");
            for (var i = 0; i < count; ++i)
            {
                var fillIn = slotState.TakeOut();
                if (fillIn != Entity.Null)
                    EntityManager.AddComponentData(fillIn, new Despawn());
            }
            //Despawn
            EntityManager.AddComponentData(entity, new Despawn());

        }

        private ushort CalculateScore(Entity product)
        {
            EntityType productType;
            if (product == Entity.Null)
            {
                productType = EntityType.None;
            }
            else
            {
                var gameEntity = EntityManager.GetComponentData<GameEntity>(product);
                productType = gameEntity.Type;
            }

            var query = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(MenuItem)
                }
            });

            ushort score = 0;
            var entities = query.ToEntityArray(Allocator.TempJob);

            var entityList = entities.ToList();
            entityList.Sort((a, b) =>
            {
                var sa = EntityManager.GetComponentData<MenuItem>(a).Index;
                var sb = EntityManager.GetComponentData<MenuItem>(b).Index;

                return sa.CompareTo(sb);
            });

            foreach (var menuEntity in entityList)
            {
                var menu = EntityManager.GetComponentData<MenuItem>(menuEntity);
                
                if (IsMatch(menu,productType))
                {
                    EntityManager.AddComponentData(menuEntity, new Despawn());
                    score = (ushort) (menu.MaterialCount() * 50);
                    break;
                }
            }

            entities.Dispose();
            return score;
        }

     

        private bool IsMatch(MenuItem menuItem, EntityType productType)
        {
            return (EntityType) menuItem.ProductId == productType;
        }

        private void AddScore(ushort value)
        {
            var query = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(Score)
                }
            });
            var entities = query.ToEntityArray(Allocator.TempJob);
            if (entities.Length < 1)
                return;

            var scoreEntity = entities[0];
            var score = EntityManager.GetComponentData<Score>(scoreEntity);
            score.Value += value;
            EntityManager.SetComponentData(scoreEntity, score);
            entities.Dispose();
        }
    }
}