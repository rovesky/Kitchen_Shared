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
                var e = EntityManager.CreateEntity(typeof(ReplicatedEntityData), typeof(Countdown), typeof(Score));
                EntityManager.SetComponentData(e, new ReplicatedEntityData()
                {
                    Id = -1,
                    PredictingPlayerId = -1
                });

                EntityManager.SetComponentData(e, new Countdown()
                {
                    Value = spawnGame.TotalTime,
                    EndTime = DateTime.Now.AddSeconds( spawnGame.TotalTime).Ticks
                });

                EntityManager.SetComponentData(e, new Score()
                {
                    Value =  spawnGame.Score
                });
            }

            array.Dispose();
        }
    }
}