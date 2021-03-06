﻿using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;


namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnItemsSystem : ComponentSystem
    {

        protected override void OnCreate()
        {
            base.OnCreate();

            var entity = EntityManager.CreateEntity(typeof(SpawnItemArray));
            SetSingleton(new SpawnItemArray());
            EntityManager.AddBuffer<SpawnItemRequest>(entity);
        }

        protected override void OnUpdate()
        {
          
            var entity = GetSingletonEntity<SpawnItemArray>();
            var requests = EntityManager.GetBuffer<SpawnItemRequest>(entity);

          //  FSLog.Info($"Spwan item :{requests.Length}");
            if (requests.Length == 0)
                return;

            var array = requests.ToNativeArray(Allocator.Temp);
            requests.Clear();

            foreach (var t in array)
            {
                var spawnItem = t;
                if (spawnItem.DeferFrame > 0)
                {
                    spawnItem.DeferFrame = spawnItem.DeferFrame - 1;
                    requests.Add(spawnItem);
                    continue;
                }

                FSLog.Info($"Spawn item:{spawnItem.Type}");


                var e = ItemCreateUtilities.CreateItem(EntityManager, spawnItem.Type,
                    spawnItem.OffPos, spawnItem.Owner);

                if (e == Entity.Null)
                    continue;

                if (spawnItem.Owner == Entity.Null)
                    continue;

                ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                    e, spawnItem.Owner, Entity.Null);


            }

            array.Dispose();
        }
    }
}