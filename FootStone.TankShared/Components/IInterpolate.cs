using System;
using System.IO;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public interface IInterpolate<T>
    {
        void Interpolate(ref T prevState, ref T nextState, float interpVal);
    }

}