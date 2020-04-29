using Unity.Entities;

namespace FootStone.Kitchen
{

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