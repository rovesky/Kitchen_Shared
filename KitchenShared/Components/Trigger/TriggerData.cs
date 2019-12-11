using System;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
	[Flags]
	public enum TriggerVolumeType
	{
		None = 0,
		Table = 1 << 1,
	}

	public struct TriggerData : IComponentData
	{
		public int VolumeType;
        public float3 SlotPos;
    }
}
