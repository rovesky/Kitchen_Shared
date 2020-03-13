using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct ItemAttachToCharacterRequest : IComponentData
    {
        public Entity Owner;
        public int PredictingPlayerId;
    }

    public struct ItemDetachFromCharacterRequest : IComponentData
    {
        public float3 Pos;
        public float3 LinearVelocity;
    }

    public struct ItemAttachToTableRequest : IComponentData
    {
        public float3 SlotPos;
    }

    public struct ItemDetachFromTableRequest : IComponentData
    {
    }
}