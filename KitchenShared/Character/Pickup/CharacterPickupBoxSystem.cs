using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupBoxSystem : SystemBase 
    {
     //   private Entity applePrefab;

        protected override void OnCreate()
        {
            //applePrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
            //    Resources.Load("Apple") as GameObject,
            //    GameObjectConversionSettings.FromWorld(World,
            //        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));
           // networkServerSystem = World.GetExistingSystem<NetworkServerSystem>();
        }

        private EntityType BoxTypeToEntityType(BoxType boxType)
        {
            switch (boxType)
            {
                case BoxType.Shrimp:
                    return EntityType.Shrimp;
                case BoxType.Rice :
                    return EntityType.Rice;
                case BoxType.Kelp :
                    return EntityType.KelpSlice;
                case BoxType.Cucumber :
                    return EntityType.Cucumber;
                default:
                    return EntityType.Shrimp;
            }
        }

        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterPickupTable")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    int entityInQueryIndex,
                    ref PickupPredictedState pickupState,
                    in PickupSetting setting,
                    in TriggerPredictedState triggerState,
                    in ReplicatedEntityData replicatedEntityData,
                    in UserCommand command) =>
                {

                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;

                    if (pickupState.PickupedEntity != Entity.Null)
                        return;

                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    if(!EntityManager.HasComponent<BoxSetting>(triggerEntity))
                        return;

                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);
                    if(slot.FilledInEntity != Entity.Null)
                        return;

                    var slotSetting = EntityManager.GetComponentData<SlotSetting>(triggerEntity);

                    var boxSetting = EntityManager.GetComponentData<BoxSetting>(triggerEntity);

                    var spawnFoodEntity = GetSingletonEntity<SpawnFoodArray>();
                    var buffer = EntityManager.GetBuffer<SpawnFoodRequest>(spawnFoodEntity);

                    var isSlice = boxSetting.Type == BoxType.Kelp;

                    buffer.Add(new SpawnFoodRequest()
                    {
                        Type = BoxTypeToEntityType(boxSetting.Type),
                        Pos = slotSetting.Pos,
                        Owner = entity,
                        IsSlice = isSlice
                    });


                }).Run();
        }

    }
}