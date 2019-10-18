using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Assets.Scripts.ECS
{
    [Serializable]
    public struct Connection : IComponentData 
    {
        public int id;
        public int sessionId;
    }
}
