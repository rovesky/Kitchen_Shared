using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    public struct PickupSetting : IComponentData
    {

    }

    public struct PickupPredictedState : IComponentData, IPredictedState<PickupPredictedState>
    {
        public Entity PickupedEntity;

        public void Deserialize(ref SerializeContext context, ref NetworkReader reader)
        {
            context.RefSerializer.DeserializeReference(ref reader, ref PickupedEntity);
        }

        public void Serialize(ref SerializeContext context, ref NetworkWriter writer)
        {
            context.RefSerializer.SerializeReference(ref writer, "pickupedEntity", PickupedEntity);
        }

        public bool VerifyPrediction(ref PickupPredictedState state)
        {
            return PickupedEntity.Equals(state.PickupedEntity);
        }

        public static IPredictedStateSerializerFactory CreateSerializerFactory()
        {
            return new PredictedStateSerializerFactory<PickupPredictedState>();
        }
    }


    public struct AttachToCharacterRequest : IComponentData
    {
        public Entity Owner;
        public int PredictingPlayerId;

    }
    public struct DetachFromCharacterRequest : IComponentData
    {
        public float3 Pos;
    }

}