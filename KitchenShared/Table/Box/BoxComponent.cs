using System;
using Unity.Entities;

namespace FootStone.Kitchen
{

    /// <summary>
    /// 食物箱桌子
    /// </summary>
    public struct TableBox : IComponentData
    {
        public EntityType Type;
    }

    public struct BoxOpenRequest: IComponentData
    {
    
    }
}