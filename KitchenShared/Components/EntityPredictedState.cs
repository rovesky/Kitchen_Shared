using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace FootStone.Kitchen
{
    public struct EntityPredictedState : IComponentData, IPredictedState<EntityPredictedState>
    {
        public RigidTransform Transform;
        public PhysicsVelocity Velocity;
     

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            Transform.pos = reader.ReadVector3Q();
            Transform.rot = reader.ReadQuaternionQ();
            Velocity.Linear = reader.ReadVector3Q();
            Velocity.Angular = reader.ReadVector3Q();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteVector3Q("position", Transform.pos);
            writer.WriteQuaternionQ("rotation", Transform.rot);
            writer.WriteVector3Q("linearVelocity", Velocity.Linear);
            writer.WriteVector3Q("angularVelocity", Velocity.Angular);
        }

        public bool VerifyPrediction(ref EntityPredictedState state)
        {
            return Transform.pos.Equals(state.Transform.pos) &&
                   Transform.rot.Equals(state.Transform.rot) &&
                   Velocity.Linear.Equals(state.Velocity.Linear) &&
                   Velocity.Angular.Equals(state.Velocity.Angular);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<EntityPredictedState>();
        }
    }
}