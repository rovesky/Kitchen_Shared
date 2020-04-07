using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public static class ItemCreateUtilities
    {
        public static void CreateItemComponent(EntityManager entityManager, Entity e, Vector3 position,
            Quaternion rotation)
        {
            entityManager.SetComponentData(e,new Translation{Value = position});
            entityManager.SetComponentData(e,new Rotation{Value = rotation});

            entityManager.AddComponentData(e, new ReplicatedEntityData
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

            entityManager.AddComponentData(e, new TransformPredictedState
            {
                Position = position,
                Rotation = rotation
            });

            entityManager.AddComponentData(e, new VelocityPredictedState
            {
                MotionType = MotionType.Static,
                Linear = float3.zero,
                Angular = float3.zero
            });

            entityManager.AddComponentData(e, new ItemPredictedState
            {
                Owner = Entity.Null,
                PreOwner = Entity.Null
            });

            entityManager.AddComponentData(e, new TriggerSetting
            {
                Distance = 0.1f
            });

            entityManager.AddComponentData(e, new TriggerPredictedState
            {
                TriggeredEntity = Entity.Null,
                IsAllowTrigger = false
            });

            entityManager.AddComponentData(e, new FoodSliceSetting()
            {
                TotalSliceTick = 150,
                OffPos = new float3(0,1.7f,0)
            });
            entityManager.AddComponentData(e, new FoodSliceState()
            {
                CurSliceTick = 0,
            //    IsSlicing = false
            });

            var compositeScale = entityManager.GetComponentData<CompositeScale>(e);
            entityManager.AddComponentData(e, new ScaleSetting()
            {
                Scale =new float3(compositeScale.Value.c0.x,compositeScale.Value.c1.y,compositeScale.Value.c2.z)
            });

            entityManager.RemoveComponent<PhysicsVelocity>(e);
        }
    }
}