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
        UnslicedBegin,
        Shrimp,
        Cucumber,
        UnslicedEnd,

        SlicedBegin,
        KelpSlice,
        ShrimpSlice,
        CucumberSlice,
        SlicedEnd,

        UnCookedBegin,
        Rice,
        UnCookedEnd,

        CookedBegin,
        RiceCooked,
        CookedEnd,
        FoodEnd,

        Game,
        Menu
    }

    public  struct GameEntity : IComponentData
    {
        public EntityType Type;
    }
}
