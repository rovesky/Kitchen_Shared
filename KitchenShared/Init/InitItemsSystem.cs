using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class InitItemsSystem : ComponentSystem
    {
        private bool isSpawned;
     
      
        protected override void OnUpdate()
        {
            if (isSpawned)
                return;

            isSpawned = true;

            ItemCreateUtilities.Init();

            var query = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(Table)
                },
                None = new ComponentType[]
                {
                    typeof(BoxSetting),
                    typeof(TableSlice)
                }
            });
            var entities = query.ToEntityArray(Allocator.TempJob);

            //生成Plate
            for (var i = 0; i < 5; ++i)
            {
                var entity = entities[i + 5];
                var slotData = EntityManager.GetComponentData<SlotSetting>(entity);

                var entityType = EntityType.Plate;
                if (i == 3)
                    entityType = EntityType.ShrimpSlice;
                if (i == 4)
                    entityType = EntityType.CucumberSlice;

                var spawnFoodEntity = GetSingletonEntity<SpawnItemArray>();
                var buffer = EntityManager.GetBuffer<SpawnItemRequest>(spawnFoodEntity);
                buffer.Add(new SpawnItemRequest()
                {
                    Type = entityType,
                   // ReplicateId = -1,
                    Pos = slotData.Pos,
                    Owner = entity
                });
            }

            entities.Dispose();
        }
     
    }
}