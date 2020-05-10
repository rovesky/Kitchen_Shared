using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [InternalBufferCapacity(16)]
    public struct SpawnItemRequest : IBufferElementData
    {
        public EntityType Type;
        public int DeferFrame;
        public float3 OffPos;
        public quaternion OffRot;
        public uint StartTick;
        public Entity Owner;

    }

    public struct SpawnItemArray : IComponentData
    {

    }
}