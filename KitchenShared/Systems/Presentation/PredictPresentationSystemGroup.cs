using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class PredictPresentationSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            AddSystemToUpdateList(World.GetOrCreateSystem<UpdateReplicatedOwnerFlag>());

            AddSystemToUpdateList(World.GetOrCreateSystem<UpdateDespawnState>());

         //   AddSystemToUpdateList(World.GetOrCreateSystem<ClearTriggeredSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<UpdateCharPresentationSystem>());
         //   AddSystemToUpdateList(World.GetOrCreateSystem<UpdateCharTriggeredSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<ApplyCharPresentationSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<ApplyItemPresentationSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<UpdateTriggeredColorSystem>());
        }
    }

}
