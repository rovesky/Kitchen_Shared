using Unity.Entities;
using FootStone.ECS;
using System;

namespace FootStone.Kitchen
{
    
    public struct PlateServedRequest : IComponentData
    {
      
    }
 

    public struct PlatePredictedState : IComponentData, IPredictedState<PlatePredictedState>
    {
        public Entity Product;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref Product);
           
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "Product", Product);
           
        }

        public bool VerifyPrediction(ref PlatePredictedState state)
        {
            return Product.Equals(state.Product);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<PlatePredictedState>();
        }
    }
}