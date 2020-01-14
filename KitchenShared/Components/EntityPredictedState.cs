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

        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
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

    public enum MotionType:byte
    {
        Static,
        Kinematic,
        Dynamic
    };

    public struct VelocityPredictedState : IComponentData, IPredictedState<VelocityPredictedState>
    {
        public float3 Linear;
        public float3 Angular;
        public MotionType MotionType;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Linear = reader.ReadVector3Q();
            Angular = reader.ReadVector3Q();
            MotionType =(MotionType)reader.ReadByte();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("Linear", Linear);
            writer.WriteVector3Q("Angular", Angular);
            writer.WriteByte("MotionType", (byte)MotionType);
        }

        public bool VerifyPrediction(ref VelocityPredictedState state)
        {
            return Linear.Equals(state.Linear) &&
                   Angular.Equals(state.Angular) &&
                   MotionType.Equals(state.MotionType);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<VelocityPredictedState>();
        }
    }
}