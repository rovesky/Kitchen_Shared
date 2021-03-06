﻿namespace FootStone.Kitchen
{
    public enum EntityType
    {
        None,
        Character,

        Plate,
        PlateDirty,
        Pot,
        Extinguisher,
  
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
 
}
