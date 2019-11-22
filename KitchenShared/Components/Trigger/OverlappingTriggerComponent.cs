using Unity.Entities;

namespace Assets.Scripts.ECS
{
	public struct OverlappingTriggerComponent : IComponentData
	{
		public int TriggerEntity;
	}
}
