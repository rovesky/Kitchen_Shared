using Unity.Entities;

namespace FootStone.Kitchen
{
    public class TriggeredSetting : IComponentData
    {
        public UnityEngine.Material OriginMaterial;
        public UnityEngine.Material TriggeredMaterial;
    }

    public struct TriggeredState : IComponentData
    {
        public bool IsTriggered;
     
    }
   
}