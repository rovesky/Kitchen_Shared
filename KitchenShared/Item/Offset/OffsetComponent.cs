using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct OffsetSetting : IComponentData
    {
        public float3 Pos;
        public quaternion Rot;
    }
}