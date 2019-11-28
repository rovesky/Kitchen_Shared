using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class CharacterFactory : ReplicatedEntityFactory
    {
        public override Entity Create(EntityManager entityManager, BundledResourceManager resourceManager,
            GameWorld world)
        {
            var playerPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Player1") as GameObject, World.Active);

            var e = entityManager.Instantiate(playerPrefab);
            Translation position = new Translation() {Value = Vector3.zero};
            Rotation rotation = new Rotation() {Value = Quaternion.identity};

            entityManager.SetComponentData(e, position);

            entityManager.AddComponentData(e, new Player());
            entityManager.AddComponentData(e, new UpdateUI());
            entityManager.AddComponentData(e, new CharacterInterpolatedState()
            {
                Position = Vector3.zero,
                Rotation = Quaternion.identity,
            });
            return e;
        }
    }
}
