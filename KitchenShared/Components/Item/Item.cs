using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{

    public struct ItemState : IComponentData
    {
        public float3 position;
        public quaternion rotation;
        public Entity owner;   
    }

    public struct ItemInterpolatedState : IComponentData, IInterpolatedState<ItemInterpolatedState>
    {
        public float3 position;
        public quaternion rotation;
        public Entity owner;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            throw new System.NotImplementedException();
        }

        public void Interpolate(ref SerializeContext context, ref ItemInterpolatedState prevState, ref ItemInterpolatedState nextState, float interpVal)
        {
            if (prevState.owner == nextState.owner)
            {
                position = Vector3.Lerp(prevState.position, nextState.position, interpVal);
                rotation = Quaternion.Lerp(prevState.rotation, nextState.rotation, interpVal);
                owner = prevState.owner;
            }
            else
            {
                position = nextState.position;
                rotation = nextState.rotation;
                owner = nextState.owner;
            }
        }

     
    }
}
