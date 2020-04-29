using System;
using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class PlateRecycleSystem : SystemBase
    {
        private const int MaxPlateCount = 4;

        private const int checkDuration = 1;
        private int lastSecond  = -1;

        protected override void OnCreate()
        {
            
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

                    var timeSpan = DateTime.Now - new DateTime(gameState.StartTime);
                    var totalSeconds = (int) timeSpan.TotalSeconds;
                    if (totalSeconds == lastSecond || totalSeconds % checkDuration != 0)
                        return;

                    lastSecond = totalSeconds;


                    var queryPlate = GetEntityQuery(new EntityQueryDesc
                    {
                        Any = new ComponentType[]
                        {
                            typeof(Plate),
                            typeof(PlateDirty)
                        }
                    });

                    var plateCount = queryPlate.CalculateEntityCount();
                    if ( plateCount>= MaxPlateCount)
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
                        Type = EntityType.PlateDirty,
                        Owner = entities[0]
                    });

                    entities.Dispose();
                }).Run();
        }
    }
}