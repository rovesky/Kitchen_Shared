using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class CatchFireBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool IsCatchFire = false;
        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new CatchFirePredictedState()
            {
                IsCatchFire = IsCatchFire
            });
        }
    }
}
