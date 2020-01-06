﻿using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public static class CreateItemUtilities
    {
        public static void CreateItemComponent(EntityManager entityManager, Entity e, Vector3 position,
            Quaternion rotation)
        {
            entityManager.SetComponentData(e,new Translation{Value = position});
            entityManager.SetComponentData(e,new Rotation{Value = rotation});

            entityManager.AddComponentData(e, new ReplicatedEntityData()
            {
                Id = 0,
                PredictingPlayerId = 0
            });

            entityManager.AddComponentData(e, new Item());

            entityManager.AddComponentData(e, new ItemInterpolatedState
            {
                Position = position,
                Rotation = Quaternion.identity,
                Owner = Entity.Null
            });

            entityManager.AddComponentData(e, new TransformPredictedState()
            {
                Position = position,
                Rotation = rotation
            });

            entityManager.AddComponentData(e, new VelocityPredictedState());

            entityManager.AddComponentData(e, new ItemPredictedState
            {
                Owner = Entity.Null,
                IsDynamic = false
            });

            entityManager.AddComponentData(e, new TriggerSetting()
            {
                Distance = 0.1f
            });

            entityManager.AddComponentData(e, new TriggerPredictedState()
            {
                TriggeredEntity = Entity.Null
            });

            entityManager.RemoveComponent<PhysicsVelocity>(e);
        }
    }
}