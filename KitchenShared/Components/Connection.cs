using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace FootStone.Kitchen
{
    [Serializable]
    public struct Connection : IComponentData 
    {
        public int id;
        public int sessionId;
    }
}
