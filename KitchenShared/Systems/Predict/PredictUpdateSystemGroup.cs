using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [UnityEngine.ExecuteAlways]
    [DisableAutoCreation]
    public class BeginPredictUpdateEntityCommandBufferSystem : EntityCommandBufferSystem {}


    [UnityEngine.ExecuteAlways]
    [DisableAutoCreation]
    public class EndPredictUpdateEntityCommandBufferSystem : EntityCommandBufferSystem {}


    [DisableAutoCreation]
    public class PredictUpdateSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            AddSystemToUpdateList(World.GetOrCreateSystem<BeginPredictUpdateEntityCommandBufferSystem>());
            
            AddSystemToUpdateList(World.GetOrCreateSystem<KitchenBuildPhysicsWorld>());
            AddSystemToUpdateList(World.GetOrCreateSystem<KitchenStepPhysicsWorld>());
            AddSystemToUpdateList(World.GetOrCreateSystem<KitchenExportPhysicsWorld>());
     
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterMoveSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<TriggerSystem>());

            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterPickupFlyingSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterPickupTableSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterPickupGroundSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterPickupPlateRecycleSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterPickupSinkSystem>());
            
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterPutDownPotSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterPutDownSinkSystem>());
           
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterDishOutSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterPickupBoxSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterDropLitterSystem>());

            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterWashSystemGroup>());

            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterThrowStartSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterThrowEndSystem>());
            
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterRushStartSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterRushEndSystem>());
            
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterServeSystem>());

            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterSlicingSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterSlicedSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<CharacterExtinguishSystemGroup>());
         
            AddSystemToUpdateList(World.GetOrCreateSystem<ItemAttachToTableSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<FoodSlicedSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<UpdatePotStateSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<UpdateExtinguisherStateSystem>());

            AddSystemToUpdateList(World.GetOrCreateSystem<UpdatePlateProductSystem>());
            
            AddSystemToUpdateList(World.GetOrCreateSystem<CookSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<BurntSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<FireSpreadSystem>());

            AddSystemToUpdateList(World.GetOrCreateSystem<UpdateFlyingStateSystem>());
            
            
            AddSystemToUpdateList(World.GetOrCreateSystem<ApplyPredictedStateSystemGroup>());
            AddSystemToUpdateList(World.GetOrCreateSystem<KitchenEndFramePhysicsSystem>());

            AddSystemToUpdateList(World.GetOrCreateSystem<EndPredictUpdateEntityCommandBufferSystem>());

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
