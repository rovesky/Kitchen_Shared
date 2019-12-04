using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class TriggerDataBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public TriggerVolumeType type = TriggerVolumeType.None;

        public GameObject slot;

        private void Awake()
        {

        }

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            var com = new TriggerDataComponent
            {
                VolumeType = (int) type
            };
            dstManager.AddComponentData(entity, com);

            if (slot != null)
            {
                var slotEntity = conversionSystem.GetPrimaryEntity(slot);
                var slotCom = new SlotComponent
                {
                    SlotEntity = slotEntity,
                    FiltInEntity = Entity.Null
                };
                dstManager.AddComponentData(entity, slotCom);
            }
        }
    }
}
