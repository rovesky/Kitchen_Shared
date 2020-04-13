using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct Score : IComponentData,IReplicatedState
    {
        public ushort Value;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Value = reader.ReadUInt16();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteUInt16("Value",Value);
        }

        public static IReplicatedStateSerializerFactory CreateSerializerFactory()
        {
            return new ReplicatedStateSerializerFactory<Score>();
        }
    }

}