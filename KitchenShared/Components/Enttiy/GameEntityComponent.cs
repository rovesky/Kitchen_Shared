using Unity.Entities;

namespace FootStone.Kitchen
{
    public  struct GameEntity : IComponentData
    {
        public EntityType Type;
    }
}