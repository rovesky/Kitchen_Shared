using Unity.Entities;

namespace FootStone.Kitchen
{
    public enum EntityType
    {
        Character,
        Plate,
        Table,

        /*foods*/
        Shrimp,
        ShrimpSlice,
        Rice,
        RiceCooked,
        KelpSlice,
        Cucumber,
        CucumberSlice,
        /*foods*/

        Game,
        Menu
    }

    public  struct GameEntity : IComponentData
    {
        public EntityType Type;
    }
}
