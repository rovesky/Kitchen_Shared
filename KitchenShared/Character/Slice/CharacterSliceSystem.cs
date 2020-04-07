using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterSetSliceSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref SlicePredictedState sliceState,
                    in PickupPredictedState pickupState,
                    in TransformPredictedState entityPredictData,
                    in TriggerPredictedState triggerState,
                    in UserCommand command) =>
                {
                    if (pickupState.PickupedEntity != Entity.Null)
                    {
                        sliceState.IsSlicing = false;
                        return;
                    }

                    if (triggerState.TriggeredEntity == Entity.Null)
                    {
                        sliceState.IsSlicing = false;
                        return;
                    }

                    //var triggerData = EntityManager.GetComponentData<TriggeredSetting>(triggerState.TriggeredEntity);
                    //if ((triggerData.Type & (int) TriggerType.Table) == 0)
                    //{
                    //    sliceState.IsSlicing = false;
                    //    return;
                    //}

                    if(!EntityManager.HasComponent<Table>(triggerState.TriggeredEntity))
                    {
                        sliceState.IsSlicing = false;
                        return;
                    }

                    if(!EntityManager.HasComponent<TableSlice>(triggerState.TriggeredEntity))
                    {
                        sliceState.IsSlicing = false;
                        return;
                    }

                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerState.TriggeredEntity);
                    if (slot.FilledInEntity == Entity.Null)
                    {
                        sliceState.IsSlicing = false;
                        return;
                    }


                    if (!EntityManager.HasComponent<FoodSliceState>(slot.FilledInEntity))
                    {
                        sliceState.IsSlicing = false;
                        return;
                    }

                    if (!command.Buttons.IsSet(UserCommand.Button.Throw))
                        return;

                    FSLog.Info($"CharacterSetSliceSystem,sliceState.IsSlicing:{sliceState.IsSlicing}");
                    sliceState.IsSlicing = true;
                }).Run();
        }
    }

    [DisableAutoCreation]
    public class CharacterSliceSystem : SystemBase
    {
        private Entity appleSlicePrefab;

        protected override void OnCreate()
        {
            appleSlicePrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(
                Resources.Load("AppleSlice") as GameObject,
                GameObjectConversionSettings.FromWorld(World,
                    World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<ConvertToEntitySystem>().BlobAssetStore));

        }

        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref SlicePredictedState sliceState,
                    in PickupPredictedState pickupState,
                    in TransformPredictedState entityPredictData,
                    in TriggerPredictedState triggerState) =>
                {
                    if (!sliceState.IsSlicing)
                        return;

                    if (pickupState.PickupedEntity != Entity.Null)
                        return;

                    if (triggerState.TriggeredEntity == Entity.Null)
                        return;

                    //var triggerData = EntityManager.GetComponentData<TriggeredSetting>(triggerState.TriggeredEntity);
                    //if ((triggerData.Type & (int) TriggerType.Table) == 0)
                    //    return;
                    if(!EntityManager.HasComponent<Table>(triggerState.TriggeredEntity))
                        return;

                    if(!EntityManager.HasComponent<TableSlice>(triggerState.TriggeredEntity))
                        return;

                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerState.TriggeredEntity);
                    if (slot.FilledInEntity == Entity.Null)
                        return;

                    if (!EntityManager.HasComponent<FoodSliceState>(slot.FilledInEntity))
                        return;

                    var itemSliceState = EntityManager.GetComponentData<FoodSliceState>(slot.FilledInEntity);
                    var itemSliceSetting = EntityManager.GetComponentData<FoodSliceSetting>(slot.FilledInEntity);


                    if (itemSliceState.CurSliceTick < itemSliceSetting.TotalSliceTick)
                    {
                        // itemSliceState.IsSlicing = true;
                        itemSliceState.CurSliceTick++;
                        EntityManager.SetComponentData(slot.FilledInEntity, itemSliceState);
                    }
                    else
                    {
                        //  itemSliceState.IsSlicing = false;
                        sliceState.IsSlicing = false;

                        var itemPos = EntityManager.GetComponentData<LocalToWorld>(slot.FilledInEntity);
                        EntityManager.DestroyEntity(slot.FilledInEntity);

                        var e = EntityManager.Instantiate(appleSlicePrefab);
                        ItemCreateUtilities.CreateItemComponent(EntityManager, e,
                            itemPos.Position, quaternion.identity);
                        EntityManager.AddComponentData(e, new Food());
                        EntityManager.AddComponentData(e, new Slice());

                        EntityManager.SetComponentData(e, new ReplicatedEntityData
                        {
                            Id = -1,
                            PredictingPlayerId = -1
                        });

                        slot.FilledInEntity = e;
                        EntityManager.SetComponentData(triggerState.TriggeredEntity, slot);
                    }

                    FSLog.Info($"CharacterSliceSystem,itemSliceState.CurSliceTick:{itemSliceState.CurSliceTick}");
                    
                }).Run();
        }
    }
}


   
