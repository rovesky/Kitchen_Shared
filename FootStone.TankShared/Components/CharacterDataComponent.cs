using System;
using Unity.Entities;

namespace Assets.Scripts.ECS
{
	[Serializable]
	public struct CharacterDataComponent : IComponentData
	{
		public float SkinWidth;
		//public float ContactTolerance;
		//public Entity Entity;
	}
}
