using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    public struct AttachToCharacterRequest : IComponentData
    {
        public Entity Owner;
        public int PredictingPlayerId;

    }
    public struct DetachFromCharacterRequest : IComponentData
    {
        public float3 Pos;
        public float3 LinearVelocity;
    }

    public struct AttachToTableRequest : IComponentData
    {
        public Entity ItemEntity;
        public float3 SlotPos;
    }

    public struct DetachFromTableRequest : IComponentData
    {
    }
}
