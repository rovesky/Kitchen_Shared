using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class CookedBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new CookedSetting
            {
                TotalCookTick = 150,
                TotalFireAlertTick = 150
            });

            dstManager.AddComponentData(entity, new CookedPredictedState()
            {
               CurCookTick = 0,
               CurFireAlertTick = 0
            });
        }
    }
}
