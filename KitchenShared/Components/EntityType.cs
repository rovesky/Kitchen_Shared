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
      
        UnSlicedBegin,
        Shrimp,
        Cucumber,
        UnSlicedEnd,

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

        Game,
        Menu
    }

    public  struct GameEntity : IComponentData
    {
        public EntityType Type;
    }
}
