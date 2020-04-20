﻿using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class PlateRecycleBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new PlateRecycle());
        }
    }
}
