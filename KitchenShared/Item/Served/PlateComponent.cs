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
        public bool IsGenProduct;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref Product);
            IsGenProduct = reader.ReadBoolean();
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "Product", Product);
            writer.WriteBoolean("IsGenProduct",IsGenProduct);
        }

        public bool VerifyPrediction(ref PlatePredictedState state)
        {
            return Product.Equals(state.Product) &&
                IsGenProduct.Equals(state.IsGenProduct);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<PlatePredictedState>();
        }
    }
}