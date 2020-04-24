using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 正在切菜
    /// </summary>
    [DisableAutoCreation]
    public class CharacterSlicingSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref SlicePredictedState sliceState,
                    in SlotPredictedState slotState,
                    in TransformPredictedState entityPredictData,
                    in TriggerPredictedState triggerState,
                    in UserCommand command) =>
                {

                    var pickupedEntity = slotState.FilledIn;
                    if (pickupedEntity != Entity.Null)
                    {
                        sliceState.IsSlicing = false;
                        return;
                    }

                    if (triggerState.TriggeredEntity == Entity.Null)
                    {
                        sliceState.IsSlicing = false;
                        return;
                    }
                 

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
                    if (slot.FilledIn == Entity.Null)
                    {
                        sliceState.IsSlicing = false;
                        return;
                    }

                    if (EntityManager.HasComponent<Sliced>(slot.FilledIn))
                    {
                        sliceState.IsSlicing = false;
                        return;
                    }

                    if (!EntityManager.HasComponent<FoodSlicedState>(slot.FilledIn))
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

    /// <summary>
    /// 切菜完成
    /// </summary>
    [DisableAutoCreation]
    public class CharacterSlicedSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref SlicePredictedState sliceState,
                    in SlotPredictedState slotState,
                    in TransformPredictedState entityPredictData,
                    in TriggerPredictedState triggerState) =>
                {
                    if (!sliceState.IsSlicing)
                        return;

                    var pickupedEntity = slotState.FilledIn;
                    if (pickupedEntity != Entity.Null)
                        return;

                    if (triggerState.TriggeredEntity == Entity.Null)
                        return;
               
                    if (!EntityManager.HasComponent<Table>(triggerState.TriggeredEntity))
                        return;

                    if (!EntityManager.HasComponent<TableSlice>(triggerState.TriggeredEntity))
                        return;

                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerState.TriggeredEntity);
                    if (slot.FilledIn == Entity.Null)
                        return;

                    if (!EntityManager.HasComponent<FoodSlicedState>(slot.FilledIn))
                        return;

                    var itemSliceState = EntityManager.GetComponentData<FoodSlicedState>(slot.FilledIn);
                    var itemSliceSetting = EntityManager.GetComponentData<FoodSlicedSetting>(slot.FilledIn);

                    if (itemSliceState.CurSliceTick < itemSliceSetting.TotalSliceTick)
                    {
                        // itemSliceState.IsSlicing = true;
                        itemSliceState.CurSliceTick++;
                        EntityManager.SetComponentData(slot.FilledIn, itemSliceState);
                        return;
                    }

                    //  itemSliceState.IsSlicing = false;
                    sliceState.IsSlicing = false;

                    EntityManager.AddComponentData(slot.FilledIn, new FoodSlicedRequest()
                    {
                        Character = entity
                    });

                    //     FSLog.Info($"CharacterSliceSystem,itemSliceState.CurSliceTick:{itemSliceState.CurSliceTick}");

                }).Run();
        }
    }
}


   
