﻿using Unity.Entities;

namespace FootStone.Kitchen
{


    [InternalBufferCapacity(16)]
    public struct SpawnGameRequest : IBufferElementData
    {
        public ushort TotalTime;
        public ushort Score;
    }

    public struct SpawnGameArray : IComponentData
    {

    }
}