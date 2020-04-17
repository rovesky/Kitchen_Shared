using System;
using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class MenuSystem : SystemBase
    {
      //  private bool isSpawned;

        private const int Duration = 5;
        private DateTime lastTime;
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
            lastTime = DateTime.Now;
            var spawnFoodEntity = GetSingletonEntity<SpawnMenuArray>();
            var requests = EntityManager.GetBuffer<SpawnMenuRequest>(spawnFoodEntity);
            requests.Add(new SpawnMenuRequest()
            {
                Type = menuList[index],
                index = index
            });
        }

        protected override void OnUpdate()
        {
         
            var now = DateTime.Now;
            var timeSpan = DateTime.Now - lastTime;
            if ((int)timeSpan.TotalSeconds == Duration)
            {
                lastTime = now;

                var query = GetEntityQuery(new EntityQueryDesc
                {
                    All = new ComponentType[]
                    {
                        typeof(Menu)
                    }
                });
                if (query.CalculateEntityCount() < 4)
                {
                    index++;
                    var spawnFoodEntity = GetSingletonEntity<SpawnMenuArray>();
                    var requests = EntityManager.GetBuffer<SpawnMenuRequest>(spawnFoodEntity);
                    requests.Add(new SpawnMenuRequest()
                    {
                        Type = menuList[index],
                        index = index
                    });
                }
            }

            //requests.Add(new SpawnMenuRequest()
            //{
            //    Type = MenuType.Shrimp,
            //    index = 1
            //});

            
            //requests.Add(new SpawnMenuRequest()
            //{
            //    Type = MenuType.Shrimp,
            //    index = 2
            //});
        }
    }
}