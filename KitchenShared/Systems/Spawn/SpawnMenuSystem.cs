using System.Collections.Generic;
using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class SpawnMenuSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var entity = EntityManager.CreateEntity(typeof(SpawnMenuArray));
            SetSingleton(new SpawnMenuArray());
            EntityManager.AddBuffer<SpawnMenuRequest>(entity);
            MenuUtilities.Init();
        }

      

        protected override void OnUpdate()
        {
            var entity = GetSingletonEntity<SpawnMenuArray>();
            var requests = EntityManager.GetBuffer<SpawnMenuRequest>(entity);
            if (requests.Length == 0)
                return;

            var array = requests.ToNativeArray(Allocator.Temp);
            requests.Clear();
            foreach (var spawnMenu in array)
            {
                var menuTemplate = MenuUtilities.GetMenuTemplate(spawnMenu.Type);
                if (menuTemplate == MenuTemplate.Null)
                    continue;

                var e = GameCreateUtilities.CreateMenuItem(EntityManager);
                var menu = EntityManager.GetComponentData<MenuItem>(e);
                menu.Index = spawnMenu.index;
                menu.ProductId = (ushort) menuTemplate.Product;
                menu.MaterialId1 = (ushort) menuTemplate.Material1;
                menu.MaterialId2 = (ushort) menuTemplate.Material2;
                menu.MaterialId3 = (ushort) menuTemplate.Material3;
                menu.MaterialId4 = (ushort) menuTemplate.Material4;
                EntityManager.SetComponentData(e, menu);
                FSLog.Info($"Spawn Menu:{spawnMenu.Type},index:{ menu.Index }");
            }

            array.Dispose();
        }
    }
}