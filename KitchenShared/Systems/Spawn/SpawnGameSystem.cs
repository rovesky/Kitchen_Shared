using System;
using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnGameSystem : ComponentSystem
    {
        protected override void OnCreate()
        {
            var entity = EntityManager.CreateEntity(typeof(SpawnGameArray));
            SetSingleton(new SpawnGameArray());
            EntityManager.AddBuffer<SpawnGameRequest>(entity);
        }

        protected override void OnUpdate()
        {
       
            var entity = GetSingletonEntity<SpawnGameArray>();
            var requests = EntityManager.GetBuffer<SpawnGameRequest>(entity);
            if (requests.Length == 0)
                return;

            var array = requests.ToNativeArray(Allocator.Temp);
            requests.Clear();

            foreach (var spawnGame in array)
            {
                 GameCreateUtilities.CreateGame(EntityManager);
            }

            array.Dispose();
        }
    }
}