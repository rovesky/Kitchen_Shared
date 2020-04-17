using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class MenuSystem : SystemBase
    {
        private bool isSpawned;

        protected override void OnUpdate()
        {
            if (isSpawned)
                return;

            isSpawned = true;


            var spawnFoodEntity = GetSingletonEntity<SpawnMenuArray>();
            var requests = EntityManager.GetBuffer<SpawnMenuRequest>(spawnFoodEntity);
            requests.Add(new SpawnMenuRequest()
            {
                Type = MenuType.Shrimp,
                index = 0
            });

            requests.Add(new SpawnMenuRequest()
            {
                Type = MenuType.Sushi,
                index = 1
            });
        }
    }
}