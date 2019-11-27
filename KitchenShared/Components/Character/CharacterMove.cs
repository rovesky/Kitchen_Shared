using System;
using Unity.Entities;

namespace Assets.Scripts.ECS
{
	public struct CharacterMove : IComponentData
	{
		public float SkinWidth;
        public float Velocity;
        //public float ContactTolerance;
    }
}
