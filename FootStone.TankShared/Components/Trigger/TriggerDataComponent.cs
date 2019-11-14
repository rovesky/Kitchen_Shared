using System;
using Unity.Entities;

namespace Assets.Scripts.ECS
{
	[Flags]
	public enum TriggerVolumeType
	{
		None = 0,
		Portal = 1 << 0,
		ChangeMaterial = 1 << 1,
		ChangeMaterialPortal = ChangeMaterial | Portal,
	}

	public struct TriggerDataComponent : IComponentData
	{
		public int VolumeType;
	}

	public struct OnTriggerEnter : IComponentData { }
	public struct OnTriggerExit : IComponentData { }
}
