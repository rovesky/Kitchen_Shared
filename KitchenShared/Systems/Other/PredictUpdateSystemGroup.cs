using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class PredictUpdateSystemGroup : NoSortComponentSystemGroup
    {

        protected override void OnCreate()
        {
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterMoveSystem>());
            m_systemsToUpdate.Add(World.GetOrCreateSystem<CharacterPickupSystem>());

        }
    }
}
