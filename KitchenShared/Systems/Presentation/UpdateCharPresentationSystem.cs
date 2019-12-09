using FootStone.ECS;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateCharPresentationSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<ServerEntity>()
                .ForEach((Entity entity,
                    ref ReplicatedEntityData replicatedEntityData,
                    ref CharacterPredictedState predictData,
                    ref CharacterInterpolatedState interpolateData,
                    ref UserCommand command,
                    ref LocalToWorld localToWorld) =>
                {
                    interpolateData.Position = predictData.Position;
                    interpolateData.Rotation = predictData.Rotation;
                    interpolateData.SqrMagnitude = new Vector2(command.TargetDir.x, command.TargetDir.z).sqrMagnitude;
                    interpolateData.MaterialId = replicatedEntityData.Id % 4 ;
                    //if(predictData.PickupedEntity != Entity.Null)
                    //   FSLog.Info($"UpdateCharPresentationSystem,pos:{predictData.Position},localToWorld:{localToWorld.Position}");
                });
        }
    }
}