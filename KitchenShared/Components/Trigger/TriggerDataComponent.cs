using System;
using Unity.Entities;

namespace Assets.Scripts.ECS
{
	[Flags]
	public enum TriggerVolumeType
	{
		None = 0,
		Portal = 1 << 0,
		Table = 1 << 1,
	}

	public struct TriggerDataComponent : IComponentData
	{
		public int VolumeType;
	}

	public struct OnPutEntity : IComponentData
	{
		public Entity Owner;
		public Entity Goods;
	}

	public struct OnPickUpEntity : IComponentData
	{
		public Entity Owner;
	}

	#region 仅客户端使用
	public struct OnTriggerEnter : IComponentData { }
	public struct OnTriggerExit : IComponentData { }
	#endregion
}
