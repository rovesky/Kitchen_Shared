using FootStone.ECS;
using Unity.Entities;
using Unity.Physics.Systems;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class PredictUpdateSystemGroup : NoSortComponentSystemGroup
    {

        protected override void OnCreate()
        {
         
            m_systemsToUpdate.Add(World.GetOrCreateSystem<BuildPhysicsWorld>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<StepPhysicsWorld>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<KitchenExportPhysicsWorld>());
         
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterMoveSystem>());
          //  m_systemsToUpdate.Add(World.GetOrCreateSystem<ItemMoveSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterImpulseMoveSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<TriggerSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupTableSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupGroundSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterThrowSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<ItemMoveToTableSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ItemAttachToCharacterSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ItemDetachFromCharacterSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ItemAttachToTableSystem>());
             
            m_systemsToUpdate.Add(World.GetOrCreateSystem<TableFilledInItemSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyPredictedStateSystemGroup>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<KitchenEndFramePhysicsSystem>());
        }
    }
}
