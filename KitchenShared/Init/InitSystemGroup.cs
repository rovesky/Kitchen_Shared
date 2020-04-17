using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class InitSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<InitGameSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<InitItemsSystem>());
        }
    }

}
