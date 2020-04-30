using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{

  

    public struct CatchFire : IComponentData
    {

    }


    public struct CatchFirePredictedState : IComponentData, IPredictedState<CatchFirePredictedState>
    {
        public bool IsCatchFire;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            IsCatchFire = reader.ReadBoolean();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            writer.WriteBoolean("IsCatchFire",IsCatchFire);
        }

        public bool VerifyPrediction(ref CatchFirePredictedState state)
        {
            return IsCatchFire.Equals(state.IsCatchFire);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<CatchFirePredictedState>();
        }
    }
}