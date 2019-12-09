using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public static class CreateEntityUtilities
    {
        public static void CreateCharacterComponent(EntityManager entityManager, Entity e, Vector3 position,
            Quaternion rotation)
        {
            entityManager.SetComponentData(e, new Translation {Value = position});
            entityManager.SetComponentData(e, new Rotation {Value = rotation});

            entityManager.AddComponentData(e, new Character
            {
                PresentationEntity = Entity.Null
            });

            entityManager.AddComponentData(e, new ReplicatedEntityData
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            entityManager.AddComponentData(e, new CharacterPredictedState
            {
                Position = position,
                Rotation = rotation,
                TriggerEntity = Entity.Null,
                PickupedEntity = Entity.Null
            });

            entityManager.AddComponentData(e, new CharacterInterpolatedState
            {
                Position = position,
                Rotation = rotation
            });

            entityManager.AddComponentData(e, UserCommand.DefaultCommand);

            entityManager.AddComponentData(e, new CharacterMove
            {
                SkinWidth = 0.02f,
                Velocity = 7.0f
            });
            entityManager.AddComponentData(e, new CharacterPickupItem());

            entityManager.AddComponentData(e, new CharacterThrowItem
            {
                Velocity = 10.0f
            });
        }
    }
}