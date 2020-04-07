using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupBoxSystem : SystemBase 
    {
        private Entity applePrefab;

        protected override void OnCreate()
        {
            applePrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("Apple") as GameObject,
                GameObjectConversionSettings.FromWorld(World,
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));

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

                    //var triggerData = EntityManager.GetComponentData<TriggeredSetting>(triggerEntity);
                    //if ((triggerData.Type & (int) TriggerType.Table) == 0)
                    //    return;

                    if(!EntityManager.HasComponent<Table>(triggerEntity))
                        return;

                    if( !EntityManager.HasComponent<BoxSetting>(triggerEntity))
                        return;

                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);
                    if(slot.FilledInEntity != Entity.Null)
                        return;

                    var boxSetting = EntityManager.GetComponentData<BoxSetting>(triggerEntity);
                    if (boxSetting.Type == BoxType.Apple)
                    {
                        var e = EntityManager.Instantiate(applePrefab);

                        ItemCreateUtilities.CreateItemComponent(EntityManager, e,
                            new float3 {x = 0.0f, y = -10f, z = 0.0f}, quaternion.identity);

                        EntityManager.AddComponentData(e, new Food());
                      
                        EntityManager.SetComponentData(e, new ReplicatedEntityData
                        {
                            Id = -1,
                            PredictingPlayerId = -1
                        });

                        ItemAttachUtilities.ItemAttachToCharacter(EntityManager,e,entity,replicatedEntityData.PredictingPlayerId);
                        pickupState.PickupedEntity = e;
                    }

                }).Run();
        }

    }
}