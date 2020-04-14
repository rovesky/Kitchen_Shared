using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterPickupTableSystem : SystemBase
    {

        private bool IsFilledInEmpty(Entity filledInEntity)
        {
            return filledInEntity == Entity.Null;
        }

        private bool IsFilledInPlate(EntityManager entityManager, Entity filledInEntity)
        {
            return filledInEntity != Entity.Null && entityManager.HasComponent<Plate>(filledInEntity);
        }

        private bool IsSameMaterial(Entity material, Food food)
        {
            if (material == Entity.Null)
                return false;
            var food1 = EntityManager.GetComponentData<Food>(material);
            FSLog.Info($"food1.Type:{food1.Type},food.Type:{food.Type}");
            return food1.Type == food.Type;
        }

        private bool HasMaterial(PlatePredictedState plateState, Entity material)
        {

            var food = EntityManager.GetComponentData<Food>(material);

            return IsSameMaterial(plateState.Material1, food)
                   || IsSameMaterial(plateState.Material2, food)
                   || IsSameMaterial(plateState.Material3, food)
                   || IsSameMaterial(plateState.Material4, food);
        }
  




        private void PutDownItem(ref PickupPredictedState pickupState,ref SlotPredictedState slot,Entity triggerEntity)
        {
            FSLog.Info($"PutDownItem ,triggerEntity:{triggerEntity}");

            ItemAttachUtilities.ItemDetachFromCharacter(EntityManager, pickupState.PickupedEntity,
                Entity.Null, float3.zero, float3.zero);
                        

            var slotSetting = EntityManager.GetComponentData<SlotSetting>(triggerEntity);
            ItemAttachUtilities.ItemAttachToTable(EntityManager, pickupState.PickupedEntity,
                triggerEntity,slotSetting.Pos);

            slot.FilledInEntity = pickupState.PickupedEntity;
            EntityManager.SetComponentData(triggerEntity, slot);

            pickupState.PickupedEntity = Entity.Null;
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
                    //未按键返回
                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;

                    //没有触发返回
                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    //触发的不是Table返回
                    if (!EntityManager.HasComponent<Table>(triggerEntity))
                        return;

                    var worldTick = GetSingleton<WorldTime>().Tick;
                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);

                    FSLog.Info($"worldTick:{worldTick},CharacterPickupTableSystem Update," +
                               $"PickupedEntity:{pickupState.PickupedEntity}," +
                               $"triggerEntity:{triggerEntity}，slot.FiltInEntity:{slot.FilledInEntity}");

                    if (pickupState.PickupedEntity == Entity.Null && slot.FilledInEntity != Entity.Null)
                    {

                        //the item is not sliced,can't pickup
                        if (EntityManager.HasComponent<FoodSlicedState>(slot.FilledInEntity))
                        {
                            var itemSliceState = EntityManager.GetComponentData<FoodSlicedState>(slot.FilledInEntity);
                            FSLog.Info($"PickUpItem,itemSliceState.CurSliceTick:{itemSliceState.CurSliceTick}");

                            if (itemSliceState.CurSliceTick > 0)
                                return;
                        }

                        FSLog.Info($"PickUpItem,command tick:{command.RenderTick},worldTick:{worldTick}");
                        ItemAttachUtilities.ItemDetachFromTable(EntityManager, slot.FilledInEntity, triggerEntity);
                        ItemAttachUtilities.ItemAttachToCharacter(EntityManager, slot.FilledInEntity, entity,
                            replicatedEntityData.PredictingPlayerId);

                        pickupState.PickupedEntity = slot.FilledInEntity;

                        slot.FilledInEntity = Entity.Null;
                        EntityManager.SetComponentData(triggerEntity, slot);
                    }
                    else if (pickupState.PickupedEntity != Entity.Null && IsFilledInEmpty(slot.FilledInEntity))
                    {
                        PutDownItem(ref pickupState, ref slot, triggerEntity);
                    }
                    else if (pickupState.PickupedEntity != Entity.Null &&
                             IsFilledInPlate(EntityManager, slot.FilledInEntity))
                    {
                     //   FSLog.Info($"PutDownItem to plate:{slot.FilledInEntity}");
                        var pickupEntity = pickupState.PickupedEntity;
                        if (!EntityManager.HasComponent<Slice>(pickupEntity))
                            return;
                        var newTriggerEntity = slot.FilledInEntity;
                        var plateState = EntityManager.GetComponentData<PlatePredictedState>(newTriggerEntity);

                        if (HasMaterial(plateState, pickupEntity))
                            return;

                        var newSlot = EntityManager.GetComponentData<SlotPredictedState>(newTriggerEntity);
                        PutDownItem(ref pickupState, ref newSlot, newTriggerEntity);

                        plateState.FillIn(pickupEntity);

                        EntityManager.SetComponentData(newTriggerEntity, plateState);
                    }

                }).Run();
        }
    }
}