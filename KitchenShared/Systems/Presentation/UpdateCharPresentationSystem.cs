using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateCharPresentationSystem : SystemBase
    {
        private Material material;

        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((ref CharacterInterpolatedState interpolateData,
                    in TransformPredictedState transformPredictData,
                    in VelocityPredictedState velocityPredictData,
                    in TriggerPredictedState triggerPredictedData,
                  //  in UserCommand command,
                    in ReplicatedEntityData replicatedEntityData
                    ) =>
                {
                    interpolateData.Position = transformPredictData.Position;
                    interpolateData.Rotation = transformPredictData.Rotation;
                 //   interpolateData.LinearVelocity = velocityPredictData.Linear;

                    var dir = Vector3.SqrMagnitude(velocityPredictData.Linear) < 0.001f ? Vector3.zero :(Vector3) math.normalize(velocityPredictData.Linear);
                    interpolateData.SqrMagnitude = new Vector2(dir.x, dir.z).sqrMagnitude;

             //       interpolateData.SqrMagnitude = new Vector2(command.TargetDir.x, command.TargetDir.z).sqrMagnitude;
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
                    EntityManager.SetSharedComponentData(triggerEntity, volumeRenderMesh);
                    //if(predictData.PickupedEntity != Entity.Null)
                    //   FSLog.Info($"UpdateCharPresentationSystem,pos:{predictData.Position},localToWorld:{localToWorld.Position}");
                }).Run();
        }
    }
}