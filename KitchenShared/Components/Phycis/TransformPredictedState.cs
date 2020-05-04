using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct TransformPredictedState : IComponentData, IPredictedState<TransformPredictedState>
    {
        public float3 Position;
        public quaternion Rotation;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Position = reader.ReadVector3Q();
            Rotation = reader.ReadQuaternionQ();
          //  FSLog.Info($"TransformPredictedState Deserialize,enity:{context.Entity},Position:{Position}!");

        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
         //   FSLog.Info($"TransformPredictedState Serialize,enity:{context.Entity},Position:{Position}!");

            writer.WriteVector3Q("position", Position);
            writer.WriteQuaternionQ("rotation", Rotation);
        }

        public bool VerifyPrediction(ref TransformPredictedState state)
        {
            return Position.Equals(state.Position) &&
                   Rotation.Equals(state.Rotation);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<TransformPredictedState>();
        }
    }
  
}