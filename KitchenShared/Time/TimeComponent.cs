using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct Time : IComponentData,IReplicatedState
    {
        public ushort Reciprocal;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Reciprocal = reader.ReadUInt16();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteUInt16("Reciprocal",Reciprocal);
        }

        public static IReplicatedStateSerializerFactory CreateSerializerFactory()
        {
            return new ReplicatedStateSerializerFactory<Time>();
        }
    }
}