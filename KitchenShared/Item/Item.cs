using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    /// <summary>
    /// 道具标签
    /// </summary>
    public struct Item : IComponentData
    {
    }

    /// <summary>
    /// 食物标签
    /// </summary>
    public struct Food : IComponentData
    {
    }

    /// <summary>
    /// 未切好食物标签
    /// </summary>
    public struct Unsliced : IComponentData
    {
    }

    /// <summary>
    /// 已切好食物标签
    /// </summary>
    public struct Sliced : IComponentData
    {
    }

    /// <summary>
    /// 未煮食物标签
    /// </summary>
    public struct Uncooked : IComponentData
    {
    }

    /// <summary>
    /// 已煮好的食物标签
    /// </summary>
    public struct Cooked : IComponentData
    {
    }

   
    /// <summary>
    /// 能装盘的食物标签
    /// </summary>
    public struct CanDishOut : IComponentData
    {
    }

    
    /// <summary>
    /// 成品菜标签
    /// </summary>
    public struct Product : IComponentData
    {
    }

    /// <summary>
    /// 干净盘子标签
    /// </summary>
    public struct Plate : IComponentData
    {
    }

    /// <summary>
    /// 脏盘子标签
    /// </summary>
    public struct PlateDirty : IComponentData
    {
    }

    
    /// <summary>
    /// 锅标签
    /// </summary>
    public struct Pot : IComponentData
    {
    }

  

    public struct ScaleSetting : IComponentData
    {
        public float3 Scale;
    }

    public struct OffsetSetting : IComponentData
    {
        public float3 Pos;
        public quaternion Rot;
    }
}