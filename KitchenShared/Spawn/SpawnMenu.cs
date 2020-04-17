using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public enum MenuType
    {
        Shrimp,
        Sushi
    }

    [InternalBufferCapacity(16)]
    public struct SpawnMenuRequest : IBufferElementData
    {
        public MenuType Type;
        public int ReplicateId;
        public ushort index;
    }

    public struct SpawnMenuArray : IComponentData
    {

    }
}