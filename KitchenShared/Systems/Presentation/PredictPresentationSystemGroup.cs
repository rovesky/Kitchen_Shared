using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class PredictPresentationSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateReplicatedOwnerFlag>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ClearTriggeredSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateCharPresentationSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateItemPresentationSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyCharPresentationSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyCharPredictedStateSystem>());

            
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyItemPresentationSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdateTriggeredColorSystem>());
        }
    }

}
