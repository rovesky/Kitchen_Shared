using System;
using Unity.Entities;

namespace FootStone.Kitchen
{
  
    [Flags]
    public enum BoxType
    {
        None = 0,
        Shrimp = 1 << 1,
        Kelp = 2 << 1,
        Cucumber = 3 << 1,
        Rice = 4 << 1
    }

    /// <summary>
    /// 食物箱桌子
    /// </summary>
    public struct TableBox : IComponentData
    {
        public BoxType Type;
    }

    public struct BoxOpenRequest: IComponentData
    {
    
    }
}