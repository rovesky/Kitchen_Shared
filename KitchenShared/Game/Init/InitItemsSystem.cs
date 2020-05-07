using FootStone.ECS;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class InitItemsSystem : SystemBase
    {
        private bool isSpawned;


        protected override void OnUpdate()
        {
            Entities
                .WithStructuralChanges()
                .ForEach((
                    ref GameStateComponent gameState) =>
                {
                    if(gameState.State != GameState.Preparing)
                        return;

                    if(gameState.IsSceneReady)
                        return;

                    //生成Item
                    var query = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(Table),
                            typeof(SlotSetting)
                        },
                        None = new ComponentType[]
                        {
                            typeof(TableBox),
                            typeof(TableSlice),
                            typeof(FireAlertSetting)
                        }
                    });
                    var entities = query.ToEntityArray(Allocator.TempJob);

                    foreach (var e in entities)
                    {
                        var translation = EntityManager.GetComponentData<Translation>(e);

                        var entityType = EntityType.None;
                        if (translation.Value.x.Equals(-5.33f) && translation.Value.z.Equals(3.43f) ||
                            translation.Value.x.Equals(-5.33f) && translation.Value.z.Equals(1.63f) ||
                            translation.Value.x.Equals(-5.33f) && translation.Value.z.Equals(-0.17f) ||
                            translation.Value.x.Equals(0.0f) && translation.Value.z.Equals(-0.2f))
                            entityType = EntityType.Plate;
                        else if (translation.Value.x.Equals(0.0f) && translation.Value.z.Equals(7f) ||
                                 translation.Value.x.Equals(0.0f) && translation.Value.z.Equals(5.2f))
                            entityType = EntityType.RiceCooked;
                        else if (translation.Value.x.Equals(0.0f) && translation.Value.z.Equals(3.4f) ||
                                 translation.Value.x.Equals(0.0f) && translation.Value.z.Equals(1.6f))
                            entityType = EntityType.CucumberSlice;
                        else if (translation.Value.x.Equals(3.82f) && translation.Value.z.Equals(8.85f))
                            entityType = EntityType.Extinguisher;


                        if (entityType == EntityType.None)
                            continue;

                        SpawnItem(e, entityType,float3.zero, quaternion.identity);
                    }

                    entities.Dispose();

                    //生成锅
                    var queryFireCook = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(TableCook)
                        }
                    });
                    var fireCooks = queryFireCook.ToEntityArray(Allocator.TempJob);

                    for (var i = 0; i < fireCooks.Length; ++i)
                    {
                        var e = fireCooks[i];
                        SpawnItem(e, EntityType.Pot,float3.zero, quaternion.Euler(
                            math.radians(new float3(0,90,0))));
                    }

                    fireCooks.Dispose();

                    gameState.IsSceneReady = true;
                }).Run();
        }

        private void SpawnItem(Entity entity, EntityType entityType,float3 offPos,quaternion offRot)
        {
            var spawnItemEntity = GetSingletonEntity<SpawnItemArray>();
            var buffer = EntityManager.GetBuffer<SpawnItemRequest>(spawnItemEntity);
            buffer.Add(new SpawnItemRequest()
            {
                Type = entityType,
                OffPos = offPos,
                OffRot = offRot,
                Owner = entity
            });
        }
    }
}