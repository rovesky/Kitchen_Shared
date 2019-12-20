﻿using FootStone.ECS;
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
            m_systemsToUpdate.Add(World.GetOrCreateSystem<BuildPhysicsWorld>());
          //  m_systemsToUpdate.Add(World.GetOrCreateSystem<StepPhysicsWorld>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<KitchenExportPhysicsWorld>());
         
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterMoveSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ItemMoveSystem>());
            
            m_systemsToUpdate.Add(World.GetOrCreateSystem<TriggerSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupGroundSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterThrowSystem>());

       
         //   m_systemsToUpdate.Add(World.GetOrCreateSystem<ItemTriggerSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<ItemToTableSystem>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<PredictRollbackStateSystemGroup>());

            m_systemsToUpdate.Add(World.GetOrCreateSystem<KitchenEndFramePhysicsSystem>());


        }
    }
}
