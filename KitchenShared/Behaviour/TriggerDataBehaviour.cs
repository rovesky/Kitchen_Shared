using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class TriggerDataBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject Slot;
        public TriggerVolumeType Type;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            if (Slot == null)
                return;

            var slotEntity = conversionSystem.GetPrimaryEntity(Slot);
            var triggerData = new TriggerData
            {
                VolumeType = (int) Type,
                SlotPos = dstManager.GetComponentData<LocalToWorld>(slotEntity).Position
            };
            dstManager.AddComponentData(entity, triggerData);

            var slotState = new SlotPredictedState
            {
                FilledInEntity = Entity.Null
            };
            //  FSLog.Info($"SlotPos:{slotCom.SlotPos}");
            dstManager.AddComponentData(entity, slotState);
        }
    }
}