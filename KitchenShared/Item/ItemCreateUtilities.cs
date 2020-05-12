using FootStone.ECS;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public static class ItemCreateUtilities
    {
        private static Dictionary<EntityType,Entity> prefabs = new Dictionary<EntityType, Entity>();


        private static void RegisterPrefabs(EntityType type, string res)
        {
            prefabs[type] = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Item/"+res) as GameObject,
                GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld,
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));

        }

        public static void Init()
        {
            RegisterPrefabs(EntityType.Shrimp, "Shrimp");
            RegisterPrefabs(EntityType.ShrimpSlice, "ShrimpSlice");
            RegisterPrefabs(EntityType.ShrimpProduct, "ShrimpSlice");
            RegisterPrefabs(EntityType.KelpSlice, "KelpSlice");
            RegisterPrefabs(EntityType.Rice, "Rice");
            RegisterPrefabs(EntityType.RiceCooked, "RiceCooked");
            RegisterPrefabs(EntityType.Cucumber, "Cucumber");
            RegisterPrefabs(EntityType.CucumberSlice, "CucumberSlice");
            RegisterPrefabs(EntityType.Sushi, "Sushi");
            RegisterPrefabs(EntityType.Plate, "Plate");
            RegisterPrefabs(EntityType.PlateDirty, "PlateDirty");
            RegisterPrefabs(EntityType.Pot, "Pot");
            RegisterPrefabs(EntityType.Extinguisher, "Extinguisher");
        }

        public static Entity CreateItem(EntityManager entityManager,
            EntityType type, Vector3 position, Entity owner)
        {
            if (!prefabs.ContainsKey(type))
            {
                FSLog.Error($"can't find prefabs by type:{type}");
                return Entity.Null;
            }

            var e = entityManager.Instantiate(prefabs[type]);

            var translation = entityManager.GetComponentData<Translation>(e);
            var rotation = entityManager.GetComponentData<Rotation>(e);
            //   FSLog.Info($"CreateItemComponent，translation：{translation.Value}");
            entityManager.AddComponentData(e, new OffsetSetting()
            {
                Pos = translation.Value,
                Rot = rotation.Value
            });
            var newPosition = (float3) position + translation.Value;
            entityManager.SetComponentData(e, new Translation {Value = newPosition});
            entityManager.SetComponentData(e, new Rotation {Value = rotation.Value});

            entityManager.AddComponentData(e, new ReplicatedEntityData
            {
                Id = -1,
                PredictingPlayerId = -1
            });
            entityManager.AddComponentData(e, new GameEntity()
            {
                Type = type
            });

            entityManager.AddComponentData(e, new Item());

            //entityManager.AddComponentData(e, new ItemInterpolatedState
            //{
            //    Position = newPosition,
            //    Rotation = rotation.Value,
            //    Owner = Entity.Null
            //});

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

            entityManager.AddComponentData(e, new OwnerPredictedState
            {
                Owner = owner,
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

            entityManager.RemoveComponent<PhysicsVelocity>(e);

            if (IsFood(type))
                entityManager.AddComponentData(e, new Food());

            if (IsUnsliced(type))
            {
                entityManager.AddComponentData(e, new Unsliced());

                entityManager.AddComponentData(e, new ProgressSetting()
                {
                    Type = ProgressType.Slice,
                    TotalTick = 150,
                    OffPos = new float3(0, 1.7f, 0)
                });
                entityManager.AddComponentData(e, new ProgressPredictState()
                {
                    CurTick = 0
                });
            }

            if (IsSliced(type))
                entityManager.AddComponentData(e, new Sliced());

            if (IsUncooked(type))
            {
                entityManager.AddComponentData(e, new Uncooked());
            }

            if (IsCooked(type))
                entityManager.AddComponentData(e, new Cooked());

            if (CanDishOut(type))
                entityManager.AddComponentData(e, new CanDishOut());

            if (IsProduct(type))
                entityManager.AddComponentData(e, new Product());

            if(IsPlate(type))
            {
                entityManager.AddComponentData(e, new Plate());
                entityManager.AddComponentData(e, new PlatePredictedState()
                {
                    Product = Entity.Null
                });
            }

            if(IsPlateDirty(type))
            {
                entityManager.AddComponentData(e, new PlateDirty());
                
                entityManager.AddComponentData(e, new ProgressSetting()
                {
                    Type = ProgressType.Wash,
                    TotalTick = 150,
                    OffPos = new float3(0, 1.7f, 0)
                });
                entityManager.AddComponentData(e, new ProgressPredictState()
                {
                    CurTick = 0
                });
            }

            if (IsPot(type))
            {
                entityManager.AddComponentData(e, new Pot());

                entityManager.AddComponentData(e, new ProgressSetting()
                {
                    Type = ProgressType.Cook,
                    TotalTick = 150,
                    OffPos = new float3(0, -1f, 0)
                });
                entityManager.AddComponentData(e, new ProgressPredictState()
                {
                    CurTick = 0
                });
                
                entityManager.AddComponentData(e, new FireAlertSetting()
                {
                    TotalTick = 150
                });
                entityManager.AddComponentData(e, new FireAlertPredictedState()
                {
                   CurTick = 0
                });

                entityManager.AddComponentData(e, new BurntPredictedState()
                {
                    IsBurnt = false
                });
                
            }

            if (IsExtinguisher(type))
            {
                entityManager.AddComponentData(e, new Extinguisher());

                entityManager.AddComponentData(e, new ExtinguisherPredictedState()
                {
                    Distance = 0
                });
            }
            return e;
        }

        private static bool IsExtinguisher(EntityType type)
        {
            return type == EntityType.Extinguisher;
        }

        private static bool IsPot(EntityType type)
        {
            return type == EntityType.Pot;
        }

        private static bool IsPlateDirty(EntityType type)
        {
            return type == EntityType.PlateDirty;
        }

        private static bool IsProduct(EntityType type)
        {
            return type > EntityType.ProductBegin && type < EntityType.ProductEnd;
        }

        private static bool IsFood(EntityType type)
        {
            return type > EntityType.FoodBegin && type < EntityType.FoodEnd;
        }

        private static bool IsUnsliced(EntityType type)
        {
            return type > EntityType.UnslicedBegin && type < EntityType.UnslicedEnd;
        }

        private static bool IsSliced(EntityType type)
        {
            return type > EntityType.SlicedBegin && type < EntityType.SlicedEnd;
        }

        private static bool IsUncooked(EntityType type)
        {
            return type > EntityType.UncookedBegin && type < EntityType.UncookedEnd;
        }

        private static bool IsCooked(EntityType type)
        {
            return type > EntityType.CookedBegin && type < EntityType.CookedEnd;
        }

        private static bool CanDishOut(EntityType type)
        {
            return type > EntityType.CanDishOutBegin && type < EntityType.CanDishOutEnd;
        }

        private static bool IsPlate(EntityType type)
        {
            return type == EntityType.Plate;
        }
    }
}