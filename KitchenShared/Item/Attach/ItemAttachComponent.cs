using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct AttachToCharacterRequest : IComponentData
    {
        public Entity Owner;
        public int PredictingPlayerId;
    }

    public struct DetachFromCharacterRequest : IComponentData
    {
        public float3 Pos;
        public float3 LinearVelocity;
    }

    public struct AttachToTableRequest : IComponentData
    {
        public Entity ItemEntity;
        public float3 SlotPos;
    }

    public struct DetachFromTableRequest : IComponentData
    {
    }
}