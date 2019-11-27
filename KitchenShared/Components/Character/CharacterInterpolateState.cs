using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    public struct CharacterInterpolateState : IComponentData,IInterpolate<CharacterInterpolateState>
    {
        public float3 Position;
        public quaternion Rotation;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Position = reader.ReadVector3Q();
            Rotation = reader.ReadQuaternionQ();
        }     

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("Position", Position);
            writer.WriteQuaternionQ("rotation", Rotation);
        }

        public void Interpolate(ref CharacterInterpolateState prevState, ref CharacterInterpolateState nextState, float interpVal)
        {
            Position = Vector3.Lerp(prevState.Position, nextState.Position, interpVal);
            Rotation = Quaternion.Lerp(prevState.Rotation, nextState.Rotation, interpVal);

        }
    }

}