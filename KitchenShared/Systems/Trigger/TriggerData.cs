using System;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [Flags]
    public enum TriggerType
    {
        None = 0,
        Table = 1 << 1,
        Item = 1 << 2,
        Character = 1 << 3
    }

    public struct TriggerData : IComponentData
    {
        public int Type;
        public float3 SlotPos;
    }
}