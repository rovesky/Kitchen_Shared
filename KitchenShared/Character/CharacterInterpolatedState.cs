using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    public struct CharacterInterpolatedState : IComponentData, IInterpolatedState<CharacterInterpolatedState>
    {
        public float3 Position;
        public quaternion Rotation;
   
        public float SqrMagnitude;
        public byte MaterialId;
        public byte ActionId;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Position = reader.ReadVector3Q();
            Rotation = reader.ReadQuaternionQ();
            SqrMagnitude = reader.ReadFloatQ();
            MaterialId = reader.ReadByte();
            ActionId = reader.ReadByte();
           // FSLog.Info($"Deserialize,MaterialId:{MaterialId}");
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("position", Position);
            writer.WriteQuaternionQ("rotation", Rotation);
            writer.WriteFloatQ("sqrMagnitude", SqrMagnitude);
            writer.WriteByte("materialId", MaterialId);
            writer.WriteByte("ActionId", ActionId);
        }

        public void Interpolate(ref SerializeContext context, ref CharacterInterpolatedState prevState,
            ref CharacterInterpolatedState nextState, float interpVal)
        {
            Position = Vector3.Lerp(prevState.Position, nextState.Position, interpVal);
            Rotation = Quaternion.Lerp(prevState.Rotation, nextState.Rotation, interpVal);
            SqrMagnitude = math.lerp(prevState.SqrMagnitude, nextState.SqrMagnitude, interpVal);
            MaterialId = prevState.MaterialId;
            ActionId = prevState.ActionId;
        }

        public static IInterpolatedStateSerializerFactory CreateSerializerFactory()
        {
            return new InterpolatedStateSerializerFactory<CharacterInterpolatedState>();
        }
    }
}