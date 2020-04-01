using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class BoxSettingBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
        public BoxType Type;

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new BoxSetting
            {
                Type =  Type,
             
            });
        }
    }
}
