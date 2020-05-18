using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct ThrowSetting : IComponentData
    {
        public float Velocity;
        public int DelayTick;
    }

    public struct ThrowPredictState : IComponentData, IPredictedState<ThrowPredictState>
    {
        public bool IsThrowed;
        public int CurTick;
        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            IsThrowed = reader.ReadBoolean();
            CurTick = reader.ReadUInt16();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteBoolean("IsThrowed",IsThrowed);
           writer.WriteUInt16("CurTick",(ushort)CurTick);
        }

        public bool VerifyPrediction(ref ThrowPredictState state)
        {
            return IsThrowed.Equals(state.IsThrowed)
                   &&CurTick.Equals(state.CurTick);
        }
    }
}