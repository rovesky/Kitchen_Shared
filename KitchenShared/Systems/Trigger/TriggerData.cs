using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

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

    public class TriggeredSetting : IComponentData
    {
        public int Type;
        public float3 SlotPos;
        public Material OriginMaterial;
        public Material TriggeredMaterial;
    }

    public struct TriggeredState : IComponentData
    {
        public bool IsTriggered;
     
    }
}