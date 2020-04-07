using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct Item : IComponentData
    {
    
    }

    public struct ScaleSetting : IComponentData
    {
        public float3 Scale;
    }
}