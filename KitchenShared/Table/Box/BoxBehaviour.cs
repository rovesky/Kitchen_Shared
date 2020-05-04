using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class BoxBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public EntityType Type;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TableBox
            {
                Type =  Type
            });
        }
    }
}
