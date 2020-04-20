using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public enum GameState
    {
        Preparing,

        Playing,

        Ending
    }
    public struct GameStateComponent : IComponentData, IReplicatedState
    {

        public GameState State;
        public long StartTime;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            State = (GameState)reader.ReadByte();

        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteByte("State",(byte)State);

        }

        public static IReplicatedStateSerializerFactory CreateSerializerFactory()
        {
            return new ReplicatedStateSerializerFactory<GameStateComponent>();
        }

    }
}