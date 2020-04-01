using System;
using Unity.Entities;

namespace FootStone.Kitchen
{
  
    [Flags]
    public enum BoxType
    {
        None = 0,
        Apple = 1 << 1,
        Banana = 2 << 1
    }
    public struct BoxSetting : IComponentData
    {
        public BoxType Type;
    }

}