using Unity.Entities;

namespace FootStone.Kitchen
{
	public struct SlotComponent : IComponentData
    {
		// ���
        public Entity SlotEntity;
		// ����Ķ���
        public Entity FiltInEntity;
    }
}