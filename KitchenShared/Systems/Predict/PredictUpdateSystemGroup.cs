using FootStone.ECS;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class PredictUpdateSystemGroup : NoSortComponentSystemGroup
    {

        protected override void OnCreate()
        {
            //World.DestroySystem(World.GetOrCreateSystem<BuildPhysicsWorld>());
            //World.DestroySystem(World.GetOrCreateSystem<StepPhysicsWorld>());
            //World.DestroySystem(World.GetOrCreateSystem<Unity.Physics.Systems.ExportPhysicsWorld>());
            //World.DestroySystem(World.GetOrCreateSystem<Unity.Physics.Systems.EndFramePhysicsSystem>());

            //m_systemsToUpdate.Add(World.GetOrCreateSystem<BuildPhysicsWorld>());
            //m_systemsToUpdate.Add(World.GetOrCreateSystem<StepPhysicsWorld>());
            //m_systemsToUpdate.Add(World.GetOrCreateSystem<MyExportPhysicsWorld>());
            //m_systemsToUpdate.Add(World.GetOrCreateSystem<EndFramePhysicsSystem>());
         
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterMoveSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterTriggerSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupGroundSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterThrowSystem>());
        

        }
    }
}
