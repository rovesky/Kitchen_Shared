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
        public static bool IsSlice(EntityType type)
        {
            if (type == EntityType.CucumberSlice 
                || type == EntityType.KelpSlice
                || type == EntityType.ShrimpSlice)
                return true;

            return false;
        }


        public static void CreateItemComponent(EntityManager entityManager, Entity e, Vector3 position,
            Quaternion rotation1)
        {
            var translation = entityManager.GetComponentData<Translation>(e);
            var rotation = entityManager.GetComponentData<Rotation>(e);
         //   FSLog.Info($"CreateItemComponent，translation：{translation.Value}");
            entityManager.AddComponentData(e,new OffsetSetting()
            {
                Pos = translation.Value,
                Rot = rotation.Value
            });
            var newPosition = (float3) position + translation.Value;
            entityManager.SetComponentData(e,new Translation{Value = newPosition});
            entityManager.SetComponentData(e,new Rotation{Value = rotation.Value});

            entityManager.AddComponentData(e, new ReplicatedEntityData
            {
                Id = 0,
                PredictingPlayerId = 0
            });

            entityManager.AddComponentData(e, new Item());

            entityManager.AddComponentData(e, new ItemInterpolatedState
            {
                Position = newPosition,
                Rotation = rotation.Value,
                Owner = Entity.Null
            });

            entityManager.AddComponentData(e, new TransformPredictedState
            {
                Position = newPosition,
                Rotation = rotation.Value
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

            entityManager.AddComponentData(e, new FoodSlicedSetting()
            {
                TotalSliceTick = 150,
                OffPos = new float3(0,1.7f,0)
            });
            entityManager.AddComponentData(e, new FoodSlicedState()
            {
                CurSliceTick = 0
            });

            if (entityManager.HasComponent<CompositeScale>(e))
            {
                var compositeScale = entityManager.GetComponentData<CompositeScale>(e);
                entityManager.AddComponentData(e, new ScaleSetting()
                {
                    Scale = new float3(compositeScale.Value.c0.x, compositeScale.Value.c1.y, compositeScale.Value.c2.z)
                });
            }
            else
            {
                entityManager.AddComponentData(e, new ScaleSetting()
                {
                    Scale = new float3(1.0f, 1.0f, 1.0f)
                });
            }

            entityManager.RemoveComponent<PhysicsVelocity>(e);
        }
    }
}