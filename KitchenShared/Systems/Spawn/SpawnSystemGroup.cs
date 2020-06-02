using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class SpawnSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            AddSystemToUpdateList(World.GetOrCreateSystem<SpawnGameSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<SpawnMenuSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<SpawnItemsSystem>());
       
        }
    }

}
