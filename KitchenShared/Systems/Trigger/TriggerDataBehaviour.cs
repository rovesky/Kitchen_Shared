using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class TriggerDataBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject Slot;
        public TriggerType Type;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            var slotPos = float3.zero;
            if (Slot != null)
            {
                var slotEntity = conversionSystem.GetPrimaryEntity(Slot);
                slotPos = dstManager.GetComponentData<LocalToWorld>(slotEntity).Position;
             
                //  FSLog.Info($"SlotPos:{slotCom.SlotPos}");
                dstManager.AddComponentData(entity, new SlotPredictedState
                {
                    FilledInEntity = Entity.Null
                });
            }
          
            var triggerData = new TriggerData
            {
                Type = (int) Type,
                SlotPos = slotPos
            };
            dstManager.AddComponentData(entity, triggerData);

          
        }
    }
}