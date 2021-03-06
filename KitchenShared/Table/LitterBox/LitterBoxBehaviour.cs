﻿using Unity.Entities;
using UnityEngine;

namespace FootStone.Kitchen
{
    public class LitterBoxBehaviour : MonoBehaviour, IConvertGameObjectToEntity
    {

        void IConvertGameObjectToEntity.Convert(Entity entity, EntityManager dstManager,
            GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new TableLitterBox());
        }
    }
}
