﻿using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class InitGameSystem : ComponentSystem
    {
        private bool isSpawned;
        public const ushort TotalTime = 300;

        protected override void OnUpdate()
        {
            if (isSpawned)
                return;

            isSpawned = true;

            var requests = EntityManager.GetBuffer<SpawnGameRequest>(GetSingletonEntity<SpawnGameArray>());
            requests.Add(new SpawnGameRequest()
            {
              //  ReplicateId = -1,
                TotalTime = TotalTime,
                Score = 0
            });
        }
    }
}