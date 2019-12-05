using Unity.Entities;
using Unity.Transforms;
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
            var com = new TriggerData
            {
                VolumeType = (int) Type
            };
            dstManager.AddComponentData(entity, com);

            if (Slot != null)
            {
                var slotEntity = conversionSystem.GetPrimaryEntity(Slot);
                var slotCom = new SlotPredictedState
                {
                    SlotPos = dstManager.GetComponentData<LocalToWorld>(slotEntity).Position,
                    FilledInEntity = Entity.Null
                };
                dstManager.AddComponentData(entity, slotCom);
            }
        }
    }
}
