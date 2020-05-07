using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ServerSystemGroup : NoSortComponentSystemGroup
    {
        protected override void OnCreate()
        {
          //  m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupBoxSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<FoodSlicedSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CountdownSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ServeSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<PlateServedSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<MenuSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<PlateRecycleSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<UpdatePlateProductSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterDishOutSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<GameStartSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<GameEndSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<GamePrepareSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterDropLitterSystem>());
     
        }
    }

}
