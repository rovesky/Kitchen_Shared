﻿using System;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    public enum Direction
    {
        Up,
        Down,
        Left,
        Right
    }

    [Serializable]
    public struct MoveTranslation : IComponentData
    {
        public float Speed;
        public Direction Direction;
    }

}