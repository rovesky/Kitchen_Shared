using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class PredictUpdateSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
       
            m_systemsToUpdate.Add(World.GetOrCreateSystem<KitchenBuildPhysicsWorld>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<KitchenStepPhysicsWorld>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<KitchenExportPhysicsWorld>());
     
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterMoveSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<TriggerSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupFlyingSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupTableSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupGroundSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupPlateRecycleSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupLitterBoxSystem>());
            
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterDishOutSystem>());
          //  m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupBoxSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterThrowSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterRushSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterServeSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterSetSliceSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterSliceSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<ItemMoveToTableSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<ApplyPredictedStateSystemGroup>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<KitchenEndFramePhysicsSystem>());
        }

        protected override void OnUpdate()
        {
            var gameQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(GameStateComponent)
                }
            });

            if (gameQuery.CalculateEntityCount() < 1)
                return;

            var gameEntities = gameQuery.ToEntityArray(Allocator.TempJob);
            var gameState = EntityManager.GetComponentData<GameStateComponent>(gameEntities[0]);
            if (gameState.State != GameState.Playing)
            {
                gameEntities.Dispose();
                return;
            }
            gameEntities.Dispose();

            base.OnUpdate();
        }
    }
}
