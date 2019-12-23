using FootStone.ECS;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateCharPresentationSystem : ComponentSystem
    {
        private Material material;

        protected override void OnUpdate()
        {
            Entities.WithAllReadOnly<ServerEntity>()
                .ForEach((ref ReplicatedEntityData replicatedEntityData,
                    ref TransformPredictedState transformPredictData,
                    ref VelocityPredictedState velocityPredictData,
                    ref TriggerPredictedState triggerPredictedData,
                    ref UserCommand command,
                    ref CharacterInterpolatedState interpolateData
                    ) =>
                {
                    interpolateData.Position = transformPredictData.Position;
                    interpolateData.Rotation = transformPredictData.Rotation;
                    interpolateData.LinearVelocity = velocityPredictData.Linear;

                    interpolateData.SqrMagnitude = new Vector2(command.TargetDir.x, command.TargetDir.z).sqrMagnitude;
                    interpolateData.MaterialId = replicatedEntityData.Id % 4 ;

                    //setup trigger entity color
                    if (triggerPredictedData.TriggeredEntity == Entity.Null)
                        return;
                    var triggerEntity = triggerPredictedData.TriggeredEntity;
                    var volumeRenderMesh = EntityManager.GetSharedComponentData<RenderMesh>(triggerEntity);

                    if (material == null)
                    {
                        material = new Material(volumeRenderMesh.material)
                        {
                            color = Color.gray
                        };
                    }
                    volumeRenderMesh.material = material;
                    PostUpdateCommands.SetSharedComponent(triggerEntity, volumeRenderMesh);
                    //if(predictData.PickupedEntity != Entity.Null)
                    //   FSLog.Info($"UpdateCharPresentationSystem,pos:{predictData.Position},localToWorld:{localToWorld.Position}");
                });
        }
    }
}