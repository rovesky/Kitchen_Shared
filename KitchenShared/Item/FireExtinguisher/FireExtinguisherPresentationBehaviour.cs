using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class FireExtinguisherPresentationBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject Slot;
      //  public GameObject Smog;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            if (Slot == null)
                return;

            var slotEntity = conversionSystem.GetPrimaryEntity(Slot);

            dstManager.AddComponentData(entity, new FireExtinguisherPresentation()
            {
               Smog = null,
               SmogPos = dstManager.GetComponentData<Translation>(slotEntity).Value,
            });

        }
    }
}
