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

            dstManager.AddComponentData(entity, new CatchFireSetting()
            {
                TotalExtinguishTick = 15,
                FireSpreadTick = 300,
                FireSpreadRadius = 1.5f,
            });

            dstManager.AddComponentData(entity, new CatchFirePredictedState()
            {
                IsCatchFire = IsCatchFire,
                CurCatchFireTick = 0,
                CurExtinguishTick = 0
            });
        }
    }
}
