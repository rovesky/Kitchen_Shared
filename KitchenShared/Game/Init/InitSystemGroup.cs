using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class InitSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            AddSystemToUpdateList(World.GetOrCreateSystem<InitGameSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<InitItemsSystem>());
        }
    }

}
