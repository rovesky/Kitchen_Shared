﻿using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class PlateServedSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity, Plate>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in PlatePredictedState plateState,
                    in OwnerPredictedState itemState,
                    in PlateServedRequest request) =>
                {

                    FSLog.Info("PlateServedSystem OnUpdate");

                    //add score
                    ushort score = CaculateScore(plateState);
                    AddScore(score);

                    EntityManager.RemoveComponent<PlateServedRequest>(entity);

                    //Despawn
                    Despawn(entity, plateState);

                    EntityManager.SetComponentData(itemState.Owner, new SlotPredictedState()
                    {
                        FilledIn = Entity.Null
                    });

                }).Run();
        }

        private void Despawn(Entity entity, PlatePredictedState plateState)
        {
            //Despawn
            EntityManager.AddComponentData(entity, new Despawn());

            if (plateState.Material1 != Entity.Null)
            {

                EntityManager.AddComponentData(plateState.Material1, new Despawn());
                plateState.Material1 = Entity.Null;
            }

            if (plateState.Material2 != Entity.Null)
            {

                EntityManager.AddComponentData(plateState.Material2, new Despawn());
                plateState.Material2 = Entity.Null;
            }

            if (plateState.Material3 != Entity.Null)
            {

                EntityManager.AddComponentData(plateState.Material3, new Despawn());
                plateState.Material3 = Entity.Null;
            }

            if (plateState.Material4 != Entity.Null)
            {

                EntityManager.AddComponentData(plateState.Material4, new Despawn());
                plateState.Material4 = Entity.Null;
            }

        }

        private ushort CaculateScore(PlatePredictedState plateState)
        {
            var query = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(Menu)
                }
            });

            ushort score = 0;
            var entities = query.ToEntityArray(Allocator.TempJob);


            for (var i = 0; i < entities.Length; ++i)
            {
                var menuEntity = entities[i];
                var menu = EntityManager.GetComponentData<Menu>(menuEntity);

                if (IsMatch(menu, ref plateState))
                {
                    EntityManager.AddComponentData(menuEntity, new Despawn());
                    score = (ushort) (menu.MaterialCount() * 50);
                    break;
                }
            }

            entities.Dispose();
            return score;
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

        private void AddScore(ushort value)
        {
            var query = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(Score)
                }
            });
            var entities = query.ToEntityArray(Allocator.TempJob);
            if (entities.Length < 1)
                return;

            var scoreEntity = entities[0];
            var score = EntityManager.GetComponentData<Score>(scoreEntity);
            score.Value += value;
            EntityManager.SetComponentData(scoreEntity, score);
            entities.Dispose();
        }
    }
}