using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class SlotBehaviour : MonoBehaviour, IConvertGameObjectToEntity
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
                Offset = float3.zero
            });

            dstManager.AddComponentData(entity, new SlotPredictedState
            {
                FilledIn = Entity.Null
            });
        }
    }
}