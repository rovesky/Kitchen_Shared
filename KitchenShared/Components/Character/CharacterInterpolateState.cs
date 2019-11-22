using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.ECS
{

    public struct CharacterInterpolateState : IComponentData,IInterpolate<CharacterInterpolateState>
    {
        public float3 position;
        public quaternion rotation;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            position = reader.ReadVector3Q();
            rotation = reader.ReadQuaternionQ();
        }     

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("position", position);
            writer.WriteQuaternionQ("rotation", rotation);
        }

        public void Interpolate(ref CharacterInterpolateState prevState, ref CharacterInterpolateState nextState, float interpVal)
        {
            position = Vector3.Lerp(prevState.position, nextState.position, interpVal);
            rotation = Quaternion.Lerp(prevState.rotation, nextState.rotation, interpVal);

        }
    }

}