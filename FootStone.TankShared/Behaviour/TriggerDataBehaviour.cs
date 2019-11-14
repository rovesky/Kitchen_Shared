using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
	public class TriggerDataBehaviour : MonoBehaviour, IConvertGameObjectToEntity
	{
		public TriggerVolumeType type = TriggerVolumeType.None;
		void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
		{
			var com = new TriggerDataComponent
			{
				VolumeType = (int)type
			};
			dstManager.AddComponentData(entity, com);
		}
	}
}
