﻿using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public static class CreateCharacterUtilities
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

            entityManager.AddComponentData(e, new TransformPredictedState()
            {
                Position = position,
                Rotation = rotation
            });
            entityManager.AddComponentData(e, new VelocityPredictedState()
            {
                MotionType = MotionType.Kinematic,
                Linear = float3.zero,
                Angular = float3.zero
            });


            entityManager.AddComponentData(e, new CharacterInterpolatedState
            {
                Position = position,
                Rotation = rotation
            });

            entityManager.AddComponentData(e, UserCommand.DefaultCommand);

            entityManager.AddComponentData(e, new CharacterMoveSetting
            {
                Gravity = PhysicsStep.Default.Gravity,
                SkinWidth = 0.02f,
                Velocity = 8.0f,
                MaxVelocity = 8.0f,
                RotationVelocity = 22.5f,
                JumpUpwardsVelocity = 4.0f,
                MaxSlope = 60.0f, 
                MaxIterations = 5,
                CharacterMass = 1.0f,
                ContactTolerance = 0.1f,
                AffectsPhysicsBodies = 1
            });

            entityManager.AddComponentData(e, new CharacterMovePredictedState
            {
               UnsupportedVelocity = float3.zero,
            });

            entityManager.AddComponentData(e, new TriggerSetting()
            {
                Distance = 0.7f
            });

            entityManager.AddComponentData(e, new TriggerPredictedState()
            {
               TriggeredEntity = Entity.Null,
               IsAllowTrigger = true
            });

            entityManager.AddComponentData(e, new PickupSetting());
            entityManager.AddComponentData(e, new PickupPredictedState
            {
                PickupedEntity = Entity.Null
            });


            entityManager.AddComponentData(e, new ThrowSetting
            {
                Velocity = 14.0f
            });
        }
    }
}