using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
   
    public class MultiSlotBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject Slot;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            if (Slot == null)
                return;

            var slotEntity = conversionSystem.GetPrimaryEntity(Slot);

            dstManager.AddComponentData(entity, new SlotSetting
            {
                Pos = dstManager.GetComponentData<Translation>(slotEntity).Value,
                Rot = dstManager.GetComponentData<Rotation>(slotEntity).Value,
                Offset = new float3(0, 0.1f, 0)

            });

            dstManager.AddComponentData(entity, new MultiSlotPredictedState
            {
                Value =
                {FilledIn1 = Entity.Null,
                FilledIn2 = Entity.Null,
                FilledIn3 = Entity.Null,
                FilledIn4 = Entity.Null}
            });
        }
    }


}