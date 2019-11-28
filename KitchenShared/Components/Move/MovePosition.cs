using System;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;


namespace FootStone.Kitchen
{
    [Serializable]
    public struct MovePosition : IComponentData
    {
        public float Speed;       
    }     

}