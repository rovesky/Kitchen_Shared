using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class TriggeredBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            var meshRenderer = gameObject.GetComponent<MeshRenderer>();

            if (meshRenderer == null)
                meshRenderer = gameObject.GetComponentInChildren<MeshRenderer>();

            if(meshRenderer == null)
                return;
         
            var material = new Material(meshRenderer.sharedMaterial);
            material.shader = Shader.Find("Custom/Outline");
     //       material.shader = Shader.Find("Wegames/Self-Illumin/Diffuse");
          //  var brightness = material.GetFloat("_Brightness");
            material.SetFloat("_Brightness", 1.4f);
            dstManager.AddComponentData(entity, new TriggeredSetting
            {
                OriginMaterial = meshRenderer.sharedMaterial,
                TriggeredMaterial = material
            });

            dstManager.AddComponentData(entity, new TriggeredState()
            {
           //     IsTriggered = false,
                TriggerEntity = Entity.Null
            });
        }
    }
}