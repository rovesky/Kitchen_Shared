using System;
using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct Countdown : IComponentData,IReplicatedState
    {
        public long EndTime;
        public ushort Value;

        public void SetValue(ushort value)
        {
            Value = value;
            EndTime = DateTime.Now.AddSeconds(Value).Ticks;
        }

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
            return new ReplicatedStateSerializerFactory<Countdown>();
        }
    }

}