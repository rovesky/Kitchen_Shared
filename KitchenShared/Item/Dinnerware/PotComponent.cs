using System;
using FootStone.ECS;
using Unity.Entities;


namespace FootStone.Kitchen
{

    public struct Plate : IComponentData
    {
    
    }

    public struct PlatePredictedState : IComponentData, IPredictedState<PlatePredictedState>
    {
        public Entity Material1;
        public Entity Material2;
        public Entity Material3;
        public Entity Material4;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref Material1);
            context.RefSerializer.DeserializeReference(ref reader, ref Material2);
            context.RefSerializer.DeserializeReference(ref reader, ref Material3);
            context.RefSerializer.DeserializeReference(ref reader, ref Material4);
           
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "Material1", Material1);
            context.RefSerializer.SerializeReference(ref writer, "Material2", Material2);
            context.RefSerializer.SerializeReference(ref writer, "Material3", Material3);
            context.RefSerializer.SerializeReference(ref writer, "Material4", Material4);
        }

        public bool VerifyPrediction(ref PlatePredictedState state)
        {
            return Material1.Equals(state.Material1) 
                && Material2.Equals(state.Material2)
                && Material3.Equals(state.Material3)
                && Material4.Equals(state.Material4);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<PlatePredictedState>();
        }

        public void FillIn(Entity entity)
        {
            if (Material1 == Entity.Null)
            {
                Material1 = entity;
            }
            else  if (Material2 == Entity.Null)
            {
                Material2 = entity;
            }
            else  if (Material3 == Entity.Null)
            {
                Material3 = entity;
            }
            else  if (Material4 == Entity.Null)
            {
                Material4 = entity;
            }
        }
    }

 
}