using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class SinkBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject SlotWashed;
        public GameObject SlotWashing;


        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            if (SlotWashed == null || SlotWashing == null)
                return;

            var slotWashedEntity = conversionSystem.GetPrimaryEntity(SlotWashed);
            var slotWashingEntity = conversionSystem.GetPrimaryEntity(SlotWashing);

            dstManager.AddComponentData(entity, new SinkSetting()
            {
                SlotWashed =  dstManager.GetComponentData<Translation>(slotWashedEntity).Value,
                SlotWashing =  dstManager.GetComponentData<Translation>(slotWashingEntity).Value,

            });

            
            dstManager.AddComponentData(entity, new SlotSetting
            {
                Pos = dstManager.GetComponentData<Translation>(slotWashedEntity).Value,
                Rot = dstManager.GetComponentData<Rotation>(slotWashedEntity).Value,
                Offset = new float3(0, 0.1f, 0)

            });
       
            dstManager.AddComponentData(entity, new SinkPredictedState()
            {
                Value =
                {
                    FilledIn1 = Entity.Null,
                    FilledIn2 = Entity.Null,
                    FilledIn3 = Entity.Null,
                    FilledIn4 = Entity.Null,
                }
            });

            dstManager.AddComponentData(entity, new MultiSlotPredictedState()
            {
                Value =
                {
                    FilledIn1 = Entity.Null,
                    FilledIn2 = Entity.Null,
                    FilledIn3 = Entity.Null,
                    FilledIn4 = Entity.Null,
                }
            });
        }
    }
}
