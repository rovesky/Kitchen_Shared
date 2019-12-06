using FootStone.ECS;
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
       
         //   dstManager.AddComponentData(entity, new ServerEntity());
            if (Slot != null)
            {
                var slotEntity = conversionSystem.GetPrimaryEntity(Slot);
                var com = new TriggerData
                {
                    VolumeType = (int)Type,
                    SlotPos = dstManager.GetComponentData<LocalToWorld>(slotEntity).Position,
                };
                dstManager.AddComponentData(entity, com);

                var slotCom = new SlotPredictedState
                {
                    FilledInEntity = Entity.Null
                };
              //  FSLog.Info($"SlotPos:{slotCom.SlotPos}");
                dstManager.AddComponentData(entity, slotCom);
            }
        }
    }
}
