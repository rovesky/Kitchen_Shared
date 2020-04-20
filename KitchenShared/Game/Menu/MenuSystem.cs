﻿using System;
using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class MenuSystem : SystemBase
    {

        private const int Duration = 5;
        private int lastSecond = -1;
        private ushort index = 0;

        private MenuType[] menuList = new MenuType[]
        {
            MenuType.Shrimp,
            MenuType.Sushi,
            MenuType.Shrimp,
            MenuType.Sushi,
            MenuType.Shrimp,
            MenuType.Sushi,
            MenuType.Shrimp,
            MenuType.Sushi,
            MenuType.Shrimp,
            MenuType.Sushi,
            MenuType.Shrimp,
            MenuType.Sushi
        };

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
                    if (gameState.State != GameState.Playing)
                        return;

                    var now = DateTime.Now;

                    var timeSpan = DateTime.Now - new DateTime(gameState.StartTime);
                    var totalSeconds = (int) timeSpan.TotalSeconds;
                    if (totalSeconds != lastSecond && totalSeconds % Duration == 0)
                    {

                        //   lastTime = now;
                        lastSecond = totalSeconds;
                        var query = GetEntityQuery(new EntityQueryDesc
                        {
                            All = new ComponentType[]
                            {
                                typeof(Menu)
                            }
                        });
                        if (query.CalculateEntityCount() < 4)
                        {
                        
                            var spawnFoodEntity = GetSingletonEntity<SpawnMenuArray>();
                            var requests = EntityManager.GetBuffer<SpawnMenuRequest>(spawnFoodEntity);
                            requests.Add(new SpawnMenuRequest()
                            {
                                Type = menuList[index],
                                index = index
                            });
                            index++;
                            FSLog.Info($"SpawnMenuRequest:{index}");
                        }
                    }
                }).Run();

        }
    }
}