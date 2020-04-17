using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct Item : IComponentData
    {
        public EntityType Type;
    }

    public struct ScaleSetting : IComponentData
    {
        public float3 Scale;
    }

    public struct OffsetSetting : IComponentData
    {
        public float3 Pos;
        public quaternion Rot;
    }
}