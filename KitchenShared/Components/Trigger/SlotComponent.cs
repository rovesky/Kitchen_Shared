using Unity.Entities;

namespace FootStone.Kitchen
{
	public struct SlotComponent : IComponentData
    {
		// 插槽
        public Entity SlotEntity;
		// 放入的对象
        public Entity FiltInEntity;
    }
}