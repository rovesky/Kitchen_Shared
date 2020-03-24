using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
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

            var meshRenderer = gameObject.GetComponent<MeshRenderer>();

        
            var triggerData = new TriggeredSetting
            {
                Type = (int) Type,
                SlotPos = slotPos,
                OriginMaterial = meshRenderer.sharedMaterial,
                TriggeredMaterial = new Material(Shader.Find("Standard")) {color = Color.gray}
            };

           
            dstManager.AddComponentData(entity, triggerData);

            dstManager.AddComponentData(entity, new TriggeredState()
            {
                IsTriggered = false
            });
        }
    }
}