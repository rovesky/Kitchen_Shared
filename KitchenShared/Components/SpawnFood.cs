using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [InternalBufferCapacity(16)]
    public struct SpawnFoodRequest : IBufferElementData
    {
        public EntityType Type;
        public float3 Pos;
        public Entity Owner;
        public bool IsSlice;
    }

    public struct SpawnFoodArray : IComponentData
    {

    }
}