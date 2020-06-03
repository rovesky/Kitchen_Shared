using System;
using System.Collections.Generic;
using Unity.Entities;


namespace FootStone.Kitchen
{
    public struct MenuTemplate : IEquatable<MenuTemplate>
    {
        public EntityType Product;
        public EntityType Material1;
        public EntityType Material2;
        public EntityType Material3;
        public EntityType Material4;

        public static MenuTemplate Null => new MenuTemplate();

        public bool Equals(MenuTemplate other)
        {
            return this ==  other;
        }

        /// <summary>
        /// Entity instances are equal if they refer to the same entity.
        /// </summary>
        /// <param name="compare">The object to compare to this Entity.</param>
        /// <returns>True, if the compare parameter contains an Entity object having the same Index and Version
        /// as this Entity.</returns>
        public override bool Equals(object compare)
        {
            return this == (MenuTemplate) compare;
        }

        /// <summary>
        /// Entity instances are equal if they refer to the same entity.
        /// </summary>
        /// <param name="lhs">An Entity object.</param>
        /// <param name="rhs">Another Entity object.</param>
        /// <returns>True, if both Index and Version are identical.</returns>
        public static bool operator ==(MenuTemplate lhs, MenuTemplate rhs)
        {
            return lhs.Product == rhs.Product 
                   && lhs.Material1 == rhs.Material1
                   && lhs.Material2 == rhs.Material2
                   && lhs.Material3 == rhs.Material3
                   && lhs.Material4 == rhs.Material4;
        }

        /// <summary>
        /// Entity instances are equal if they refer to the same entity.
        /// </summary>
        /// <param name="lhs">An Entity object.</param>
        /// <param name="rhs">Another Entity object.</param>
        /// <returns>True, if either Index or Version are different.</returns>
        public static bool operator !=(MenuTemplate lhs, MenuTemplate rhs)
        {
            return !(lhs == rhs);
        }


        public int MaterialCount()
        {
            var count = 0;

            if (Material1 != 0)
                count++;
            if (Material2 != 0)
                count++;
            if (Material3 != 0)
                count++;
            if (Material4 != 0)
                count++;

            return count;
        }

        public bool HasMaterial(EntityType material)
        {
            return Material1 == material 
                   || Material2 == material 
                   || Material3 == material
                   || Material4 == material;

        }

    }

    public static class MenuUtilities
    {
        private static Dictionary<EntityType, MenuTemplate> menuTemplates = new Dictionary<EntityType, MenuTemplate>();

        public  static void Init()
        {
         //   RegisterMenu(EntityType.ShrimpProduct,   EntityType.ShrimpSlice);
            RegisterMenu(EntityType.ShrimpSlice,   EntityType.ShrimpSlice);
            RegisterMenu(EntityType.Sushi,   EntityType.KelpSlice,
                EntityType.RiceCooked,  EntityType.CucumberSlice);

        }

        private static void RegisterMenu(EntityType product,
            EntityType material1, EntityType material2 = EntityType.None,
            EntityType material3 = EntityType.None, EntityType material4 = EntityType.None)
        {
          
            menuTemplates.Add(product, new MenuTemplate()
            {
                Product = product,
                Material1 = material1,
                Material2 = material2,
                Material3 = material3,
                Material4 = material4,
            });
        }

        private static bool HasMaterial(EntityManager entityManager,MenuTemplate template, Entity entity)
        {
            if (entity == Entity.Null)
                return false;

            var gameEntity = entityManager.GetComponentData<GameEntity>(entity);
            return template.HasMaterial(gameEntity.Type);

        }

        private static bool IsMatch(EntityManager entityManager,MenuTemplate menuTemplate,MultiSlotPredictedState plateState)
        {
            if (plateState.Value.Count() != menuTemplate.MaterialCount())
                return false;

            if (plateState.Value.Count() == 1)
                return HasMaterial(entityManager, menuTemplate, plateState.Value.FilledIn1);

            if (plateState.Value.Count() == 2)
                return HasMaterial(entityManager, menuTemplate, plateState.Value.FilledIn1) &&
                       HasMaterial(entityManager, menuTemplate, plateState.Value.FilledIn2);

            if (plateState.Value.Count() == 3)
                return HasMaterial(entityManager, menuTemplate, plateState.Value.FilledIn1) &&
                       HasMaterial(entityManager, menuTemplate, plateState.Value.FilledIn2)&&
                       HasMaterial(entityManager, menuTemplate, plateState.Value.FilledIn3);

            if (plateState.Value.Count() == 4)
                return HasMaterial(entityManager, menuTemplate, plateState.Value.FilledIn1) &&
                       HasMaterial(entityManager, menuTemplate, plateState.Value.FilledIn2) &&
                       HasMaterial(entityManager, menuTemplate, plateState.Value.FilledIn3) &&
                       HasMaterial(entityManager, menuTemplate, plateState.Value.FilledIn4);

            return true;
        }


        public static MenuTemplate MatchMenuTemplate(EntityManager entityManager,MultiSlotPredictedState plateState)
        {
            foreach (var menuTemplate in menuTemplates.Values)
            {
                if (IsMatch(entityManager, menuTemplate, plateState))
                    return menuTemplate;
            }
          
            return  MenuTemplate.Null;

        }

        public static MenuTemplate GetMenuTemplate(EntityType type)
        {
            return !menuTemplates.ContainsKey(type) ? MenuTemplate.Null : menuTemplates[type];
        }
    }

}