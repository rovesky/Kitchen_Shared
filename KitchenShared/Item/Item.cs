using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    /// <summary>
    /// ���߱�ǩ
    /// </summary>
    public struct Item : IComponentData
    {
    }

    /// <summary>
    /// ʳ���ǩ
    /// </summary>
    public struct Food : IComponentData
    {
    }

    /// <summary>
    /// δ�к�ʳ���ǩ
    /// </summary>
    public struct Unsliced : IComponentData
    {
    }

    /// <summary>
    /// ���к�ʳ���ǩ
    /// </summary>
    public struct Sliced : IComponentData
    {
    }

    /// <summary>
    /// δ��ʳ���ǩ
    /// </summary>
    public struct Uncooked : IComponentData
    {
    }

    /// <summary>
    /// ����õ�ʳ���ǩ
    /// </summary>
    public struct Cooked : IComponentData
    {
    }

   
    /// <summary>
    /// ��װ�̵�ʳ���ǩ
    /// </summary>
    public struct CanDishOut : IComponentData
    {
    }

    
    /// <summary>
    /// ��Ʒ�˱�ǩ
    /// </summary>
    public struct Product : IComponentData
    {
    }

    /// <summary>
    /// �ɾ����ӱ�ǩ
    /// </summary>
    public struct Plate : IComponentData
    {
    }

    /// <summary>
    /// �����ӱ�ǩ
    /// </summary>
    public struct PlateDirty : IComponentData
    {
    }

    
    /// <summary>
    /// ����ǩ
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