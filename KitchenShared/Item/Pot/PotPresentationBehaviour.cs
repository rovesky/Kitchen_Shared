using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class PotPresentationBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject Full;
        public GameObject Empty;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {

         //   var steam = Instantiate(Resources.Load("Steam")) as GameObject;
         //   steam.SetActive(true);


            dstManager.AddComponentData(entity, new PotPresentation()
            {
                Full = conversionSystem.GetPrimaryEntity(Full),
                Empty = conversionSystem.GetPrimaryEntity(Empty),
                Steam = null
            });

        }
    }
}
