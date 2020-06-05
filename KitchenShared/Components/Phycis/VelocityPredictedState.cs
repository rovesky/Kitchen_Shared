using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{

    public enum MotionType:byte
    {
        Static,
        Kinematic,
        Dynamic
    }

    public struct VelocityPredictedState : IComponentData, IPredictedState<VelocityPredictedState>
    {
        public float3 Linear;
        public float3 Angular;
        public MotionType MotionType;
        public float SqrMagnitude;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Linear = reader.ReadVector3Q();
            Angular = reader.ReadVector3Q();
            MotionType =(MotionType)reader.ReadByte();

            var dir = Vector3.SqrMagnitude(Linear) < 0.001f? Vector3.zero: (Vector3) math.normalize(Linear);
            SqrMagnitude = new Vector2(dir.x, dir.z).sqrMagnitude;

      
          //  FSLog.Info($"VelocityPredictedState Deserialize,enity:{context.Entity},Linear:{Linear}!");

           // FSLog.Info($"VelocityPredictedState,Linear:{Linear}");
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("Linear", Linear);
            writer.WriteVector3Q("Angular", Angular);
            writer.WriteByte("MotionType", (byte)MotionType);
        }

        public bool VerifyPrediction(ref VelocityPredictedState state)
        {
            return Vector3.SqrMagnitude(Linear - state.Linear) < 0.025f &&
                   Vector3.SqrMagnitude(Angular - state.Angular) < 0.025f &&
                   MotionType.Equals(state.MotionType);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<VelocityPredictedState>();
        }
    }
}