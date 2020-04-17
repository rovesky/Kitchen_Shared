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

            var material = new UnityEngine.Material(meshRenderer.sharedMaterial);
            material.SetFloat("_Brightness", 1.4f);
            dstManager.AddComponentData(entity, new TriggeredSetting
            {
                OriginMaterial = meshRenderer.sharedMaterial,
                TriggeredMaterial = material
            });

            dstManager.AddComponentData(entity, new TriggeredState()
            {
                IsTriggered = false
            });
        }
    }
}