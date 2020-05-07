using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct Character : IComponentData
    {
        public Entity PresentationEntity;
    }

    
    public struct LocalCharacter : IComponentData
    {
     
    }
}