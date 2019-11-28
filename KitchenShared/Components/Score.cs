using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [Serializable]
    public struct Score : IComponentData 
    {
        public int ScoreValue;

        public int MaxScoreValue;
    }
}
