using Unity.Entities;

namespace FootStone.Kitchen
{
    public struct Score : IComponentData
    {
        public int ScoreValue;

        public int MaxScoreValue;
    }
}