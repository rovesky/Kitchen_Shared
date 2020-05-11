using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class SpawnPointBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {
      


        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new SpawnPoint());
        }
    }
}
