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
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupSinkSystem>());
            
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPutDownPotSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPutDownSinkSystem>());
           
          //  m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterDishOutSystem>());
          //  m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupBoxSystem>());
         // m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterDropLitterSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterWashSystemGroup>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterThrowSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterRushSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterServeSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterSlicingSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterSlicedSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterExtinguishSystemGroup>());
       

            m_systemsToUpdate.Add(World.GetOrCreateSystem<ItemMoveToTableSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CookSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CatchFireSystem>());
            

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
