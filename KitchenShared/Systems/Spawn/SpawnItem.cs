using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [InternalBufferCapacity(16)]
    public struct SpawnItemRequest : IBufferElementData
    {
        public EntityType Type;
      //  public int ReplicateId;
        public float3 Pos;
        public Entity Owner;

    }

    public struct SpawnItemArray : IComponentData
    {

    }
}