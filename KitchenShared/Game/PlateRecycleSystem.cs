using System;
using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class PlateRecycleSystem : SystemBase
    {
        private const int MaxPlateCount = 4;

        private const int checkDuration = 10;
        private DateTime lastCheckTime;

        protected override void OnCreate()
        {
            lastCheckTime = DateTime.Now;
        }

        protected override void OnUpdate()
        {

            Entities
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in GameStateComponent gameState) =>
                {
                    if(gameState.State != GameState.Playing)
                        return;

                    var now = DateTime.Now;
                    var timeSpan = DateTime.Now - lastCheckTime;
                    if ((int) timeSpan.TotalSeconds != checkDuration)
                        return;
                    lastCheckTime = now;


                    var queryPlate = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(Plate)
                        }
                    });

                    if (queryPlate.CalculateEntityCount() >= MaxPlateCount)
                        return;

                    var queryTable = GetEntityQuery(new EntityQueryDesc
                    {
                        All = new ComponentType[]
                        {
                            typeof(PlateRecycle)
                        }
                    });
                    if (queryTable.CalculateEntityCount() < 1)
                        return;


                    var entities = queryTable.ToEntityArray(Allocator.TempJob);


                    var spawnItemEntity = GetSingletonEntity<SpawnItemArray>();
                    var buffer = EntityManager.GetBuffer<SpawnItemRequest>(spawnItemEntity);
                    buffer.Add(new SpawnItemRequest()
                    {
                        Type = EntityType.Plate,
                        Owner = entities[0]
                    });

                    entities.Dispose();
                }).Run();
        }
    }
}