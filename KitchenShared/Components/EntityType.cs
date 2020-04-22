using Unity.Entities;

namespace FootStone.Kitchen
{
    public enum EntityType
    {
        None,
        Character,

        Plate,
        PlateDirty,
        PotEmpty,
        PotFull,
        FireExtinguisher,
  
        LitterBox,
        Table,
      
        FoodBegin,
        CannotDishOutBegin,
        UnslicedBegin,
        Shrimp,
        Cucumber,
        UnslicedEnd,

        UncookedBegin,
        Rice,
        UncookedEnd,
        CannotDishOutEnd,

        CanDishOutBegin,
        SlicedBegin,
        KelpSlice,
        ShrimpSlice,
        CucumberSlice,
        SlicedEnd,

        CookedBegin,
        RiceCooked,
        CookedEnd,
        CanDishOutEnd,

        ProductBegin,
        ShrimpProduct,
        Sushi,
        ProductEnd,


        FoodEnd,


        Game,
        Menu
    }

    public  struct GameEntity : IComponentData
    {
        public EntityType Type;
    }
}
