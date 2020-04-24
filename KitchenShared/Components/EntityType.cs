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
  
        TableBegin,
        Table,
        LitterBox,
        TablePlateRecycle,
        FoodBox,
        TableSlice,
        TableSink,
        TableCook,
        TableServe,
        TableEnd,

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
