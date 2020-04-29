using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class PresentationBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject PresentationObject;


        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            var presentationEntity = PresentationObject.GetComponentInChildren<GameObjectEntity>() == null
                ? Entity.Null
                : PresentationObject.GetComponentInChildren<GameObjectEntity>().Entity;
            
            dstManager.AddComponentData(entity, new Presentation
            {
                Value = presentationEntity
            });
        }
    }
}
