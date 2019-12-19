using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class PredictRollbackStateSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ItemUpdatePredictedStateSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<EntityUpdatePredictedStateSystem>());
        }
    }

}
