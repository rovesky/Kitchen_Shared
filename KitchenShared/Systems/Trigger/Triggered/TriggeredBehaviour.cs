using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class TriggeredBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
       // public GameObject Slot;
       // public TriggerType Type;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();

            dstManager.AddComponentData(entity, new TriggeredSetting
            {
             //   Type = (int) Type,
                //     SlotPos = slotPos,
                OriginMaterial = meshRenderer.sharedMaterial,
                TriggeredMaterial = new Material(Shader.Find("Standard")) {color = Color.gray}
            });

            dstManager.AddComponentData(entity, new TriggeredState()
            {
                IsTriggered = false
            });
        }
    }
}