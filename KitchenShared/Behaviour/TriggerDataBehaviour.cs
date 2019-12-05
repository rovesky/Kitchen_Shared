using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class TriggerDataBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public TriggerVolumeType Type = TriggerVolumeType.None;

        public GameObject Slot;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            var com = new TriggerDataComponent
            {
                VolumeType = (int) Type
            };
            dstManager.AddComponentData(entity, com);

            if (Slot != null)
            {
                var slotEntity = conversionSystem.GetPrimaryEntity(Slot);
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
