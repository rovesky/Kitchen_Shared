using System;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct Enemy : IComponentData
    {
        public EnemyType type;
        public int id;
    }

}