using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class ApplyCharPresentationSystem : ComponentSystem
    {
        private Material material;

        protected override void OnCreate()
        {
           
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,
                ref CharacterPredictedState predictedState,
                ref CharacterInterpolatedState interpolatedData,
                ref Translation translation,
                ref Rotation rotation) =>
            {
                translation.Value = interpolatedData.Position;
                translation.Value.y = 1.0f;
                rotation.Value = interpolatedData.Rotation;

                //setup trigger entity color
                if (predictedState.TriggeredEntity == Entity.Null)
                    return;
                var triggerEntity = predictedState.TriggeredEntity;
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


                //  FSLog.Info($"ApplyCharPresentationSystem,x:{predictData.Position.x},z:{predictData.Position.z}," +
                //       $"translation.Value.x:{ translation.Value.x},translation.Value.z:{ translation.Value.z}");
            });
        }
    }
}