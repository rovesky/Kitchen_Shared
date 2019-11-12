using System;
using Unity.Entities;

namespace Assets.Scripts.ECS
{
	public struct CharacterDataComponent : IComponentData
	{
		public float SkinWidth;
		public Entity Entity;
		//public float ContactTolerance;
	}
}
