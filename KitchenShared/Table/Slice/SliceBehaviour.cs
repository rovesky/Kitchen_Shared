using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class SliceBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public GameObject Knife;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TableSlice()
            {
                Knife = conversionSystem.GetPrimaryEntity(Knife)
            });
        }
    }
}
