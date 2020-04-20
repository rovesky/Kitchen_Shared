using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterDishOutSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities
                .WithAll<ServerEntity>()
                .WithName("CharacterDishOutSystem")
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in TriggerPredictedState triggerState,
                    in SlotPredictedState slotState,
                    in UserCommand command) =>
                {
                    //未按键返回
                    if (!command.Buttons.IsSet(UserCommand.Button.Pickup))
                        return;

                    //没有拾取返回
                    var pickupEntity = slotState.FilledIn;
                    if (pickupEntity == Entity.Null)
                        return;

                    if (!EntityManager.HasComponent<Material>(pickupEntity))
                        return;

                    //没有触发返回
                    var triggerEntity = triggerState.TriggeredEntity;
                    if (triggerEntity == Entity.Null)
                        return;

                    //触发的不是Table返回
                    if (!EntityManager.HasComponent<Table>(triggerEntity))
                        return;

                    //Table上没有道具返回
                    var slot = EntityManager.GetComponentData<SlotPredictedState>(triggerEntity);
                    if (slot.FilledIn == Entity.Null)
                        return;

                    //Table上不是盘子返回
                    if (!EntityManager.HasComponent<Plate>(slot.FilledIn))
                        return;

                    var plateEntity = slot.FilledIn;
                    var plateState = EntityManager.GetComponentData<PlatePredictedState>(plateEntity);

                    //盘子已满
                    if (plateState.IsFull())
                        return;
                    //盘子已有该材料
                    if (HasMaterial(plateState, pickupEntity))
                        return;

                    //改变Owner
                    ItemAttachUtilities.ItemAttachToOwner(EntityManager,
                        pickupEntity, plateEntity, entity);

                    plateState.FillIn(pickupEntity);
                    EntityManager.SetComponentData(plateEntity, plateState);


                    var productId = CaculateProduct(plateState);
                    if (productId != 0)
                    {

                    }



                }).Run();
        }


        private bool IsSameMaterial(Entity material, GameEntity food)
        {
            if (material == Entity.Null)
                return false;
            var food1 = EntityManager.GetComponentData<GameEntity>(material);
            FSLog.Info($"food1.Type:{food1.Type},food.Type:{food.Type}");
            return food1.Type == food.Type;
        }

        private bool HasMaterial(PlatePredictedState plateState, Entity material)
        {
            var food = EntityManager.GetComponentData<GameEntity>(material);

            return IsSameMaterial(plateState.Material1, food)
                   || IsSameMaterial(plateState.Material2, food)
                   || IsSameMaterial(plateState.Material3, food)
                   || IsSameMaterial(plateState.Material4, food);
        }


        private ushort CaculateProduct(PlatePredictedState plateState)
        {
            var query = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(Menu)
                }
            });

            ushort productId = 0;
            var entities = query.ToEntityArray(Allocator.TempJob);

            for (var i = 0; i < entities.Length; ++i)
            {
                var menuEntity = entities[i];
                var menu = EntityManager.GetComponentData<Menu>(menuEntity);

                if (IsMatch(menu, ref plateState))
                {
                    productId = menu.ProductId;
                    break;
                }
            }

            entities.Dispose();
            return productId;
        }

        private bool HasMaterial(Menu menu, Entity entity)
        {
            if (entity == Entity.Null)
                return false;
            var food = EntityManager.GetComponentData<GameEntity>(entity);
            return menu.HasMaterial((ushort) food.Type);
        }

        private bool IsMatch(Menu menu, ref PlatePredictedState plateState)
        {
            var plateMaterialCount = plateState.MaterialCount();
            if (menu.MaterialCount() != plateMaterialCount)
                return false;


            if (plateMaterialCount == 1)
                return HasMaterial(menu, plateState.Material1);

            if (plateMaterialCount == 2)
                return HasMaterial(menu, plateState.Material1) &&
                       HasMaterial(menu, plateState.Material2);

            if (plateMaterialCount == 3)
                return HasMaterial(menu, plateState.Material1) &&
                       HasMaterial(menu, plateState.Material2) &&
                       HasMaterial(menu, plateState.Material3);

            if (plateMaterialCount == 4)
                return HasMaterial(menu, plateState.Material1) &&
                       HasMaterial(menu, plateState.Material2) &&
                       HasMaterial(menu, plateState.Material3) &&
                       HasMaterial(menu, plateState.Material3);
            return true;
        }
    }
}