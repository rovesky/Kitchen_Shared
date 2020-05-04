using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class CookedBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new FireAlertSetting
            {
              
                TotalTick = 150
            });

            dstManager.AddComponentData(entity, new FireAlertPredictedState()
            {
               CurTick = 0
            });
        }
    }
}
