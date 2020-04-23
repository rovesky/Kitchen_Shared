using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class GameEntityBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public EntityType Type;
        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new GameEntity()
            {
                Type = Type
            });
        }
    }
}
