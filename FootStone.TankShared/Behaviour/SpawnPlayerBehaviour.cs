﻿using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{  

    public class SpawnPlayerBehaviour : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
    {
        public GameObject prefabs;
    
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem) =>
            dstManager.AddComponentData(entity, new SpawnPlayer()
            {
                entity = conversionSystem.GetPrimaryEntity(prefabs),
                spawned = false
            });

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(prefabs);
        }
    }
   
}
