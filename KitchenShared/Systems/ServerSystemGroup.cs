using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ServerSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
            AddSystemToUpdateList(World.GetOrCreateSystem<ServeSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<PlateServedSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<PlateRecycleSystem>());

            AddSystemToUpdateList(World.GetOrCreateSystem<CountdownSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<MenuSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<GameStartSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<GameEndSystem>());
            AddSystemToUpdateList(World.GetOrCreateSystem<GamePrepareSystem>());
         //   AddSystemToUpdateList(World.GetOrCreateSystem<UpdateCharPresentationSystem>());
      
        }
    }

}
