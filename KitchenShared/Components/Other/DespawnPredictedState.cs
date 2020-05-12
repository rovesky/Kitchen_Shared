using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct DespawnPredictedState : IComponentData, IPredictedState<DespawnPredictedState>
    {
        public bool IsDespawn;
        public ushort Tick;
       
        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            IsDespawn = reader.ReadBoolean();
            Tick = reader.ReadUInt16();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteBoolean("IsDespawn",IsDespawn);
            writer.WriteUInt16("Tick",Tick);
        }

        public bool VerifyPrediction(ref DespawnPredictedState state)
        {
            return IsDespawn.Equals(state.IsDespawn) &&
                Tick.Equals(state.Tick);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<DespawnPredictedState>();
        }
    }
}