using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    //public enum MenuType
    //{
    //    Shrimp,
    //    Sushi
    //}

    [InternalBufferCapacity(16)]
    public struct SpawnMenuRequest : IBufferElementData
    {
        public EntityType Type;
        public ushort index;
    }

    public struct SpawnMenuArray : IComponentData
    {

    }
}