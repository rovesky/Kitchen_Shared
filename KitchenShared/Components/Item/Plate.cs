using FootStone.ECS;
using System;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{    
    
    public struct Plate : IComponentData
    {
		public bool IsFree;
    }

}