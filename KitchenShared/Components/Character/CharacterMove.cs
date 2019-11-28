using System;
using Unity.Entities;

namespace FootStone.Kitchen
{
	public struct CharacterMove : IComponentData
	{
		public float SkinWidth;
        public float Velocity;
        //public float ContactTolerance;
    }
}
