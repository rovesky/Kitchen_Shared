using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct RushSetting : IComponentData
    {
        public float Velocity;
        public ushort DurationTick;
        public ushort CooldownTick;
    }

    public struct RushPredictState : IComponentData, IPredictedState<RushPredictState>
    {
        public bool IsRushed;
        public int CurCooldownTick;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            IsRushed = reader.ReadBoolean();
            CurCooldownTick = reader.ReadUInt16();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteBoolean("IsRushed",IsRushed);
            writer.WriteUInt16("CurCooldownTick", (ushort) CurCooldownTick);
        }

        public bool VerifyPrediction(ref RushPredictState state)
        {
            return IsRushed.Equals(state.IsRushed)
                   && CurCooldownTick.Equals(state.CurCooldownTick);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<RushPredictState>();
        }
    }
}