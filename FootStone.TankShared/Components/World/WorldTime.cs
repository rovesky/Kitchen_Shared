using FootStone.ECS;
using System;
using System.IO;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    [Serializable]
    public struct WorldTime : IComponentData
    {
        public GameTick tick;
        public double frameTime;          
    }

}