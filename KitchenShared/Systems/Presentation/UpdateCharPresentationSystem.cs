using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateCharPresentationSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    ref CharacterInterpolatedState interpolateData,
                    in TransformPredictedState transformPredictData,
                    in VelocityPredictedState velocityPredictData,
                    in TriggerPredictedState triggerPredictedData,
                    in SlicePredictedState slicePredictedState,
                    in WashPredictedState washPredictedState,
                    in ReplicatedEntityData replicatedEntityData) =>
                {

                    if (HasSingleton<Server>())
                    {
                        SetInterpolateData(ref interpolateData, transformPredictData, replicatedEntityData,
                            slicePredictedState, washPredictedState, triggerPredictedData);
                        
                        SetInterpolateSqrMagnitude(ref interpolateData,velocityPredictData);
                    }
                    else
                    {
                        if (EntityManager.HasComponent<LocalCharacter>(entity))
                        {
                            SetInterpolateData(ref interpolateData, transformPredictData, replicatedEntityData,
                                slicePredictedState, washPredictedState, triggerPredictedData);

                            var dir = Vector3.SqrMagnitude(velocityPredictData.Linear) < 0.001f
                                ? Vector3.zero
                                : (Vector3) math.normalize(velocityPredictData.Linear);
                            interpolateData.SqrMagnitude = new Vector2(dir.x, dir.z).sqrMagnitude;
                            
                        }
                        else
                        {
                            SetInterpolateSqrMagnitude(ref interpolateData,velocityPredictData);
                        }
                    }
                }).Run();
        }

        private  void SetInterpolateSqrMagnitude(ref CharacterInterpolatedState interpolateData,
            VelocityPredictedState velocityPredictData)
        {
            if (velocityPredictData.SqrMagnitude > 0)
            {
                interpolateData.SqrMagnitude = velocityPredictData.SqrMagnitude;
            }
            else
            {
                var dir = Vector3.SqrMagnitude(velocityPredictData.Linear) < 0.001f
                    ? Vector3.zero
                    : (Vector3) math.normalize(velocityPredictData.Linear);
                interpolateData.SqrMagnitude = new Vector2(dir.x, dir.z).sqrMagnitude;
            }
        }

        private void SetInterpolateData(ref CharacterInterpolatedState interpolateData,
            TransformPredictedState transformPredictData, ReplicatedEntityData replicatedEntityData,
            SlicePredictedState slicePredictedState, WashPredictedState washPredictedState,
            TriggerPredictedState triggerPredictedData)
        {
            interpolateData.Position = transformPredictData.Position;
            interpolateData.Rotation = transformPredictData.Rotation;

            interpolateData.MaterialId = (byte) (replicatedEntityData.Id % 4);

            interpolateData.ActionId =
                (byte) (slicePredictedState.IsSlicing || washPredictedState.IsWashing ? 1 : 0);

            //setup trigger entity 
            if (triggerPredictedData.TriggeredEntity == Entity.Null)
                return;
            var triggerEntity = triggerPredictedData.TriggeredEntity;
            var triggerState = EntityManager.GetComponentData<TriggeredState>(triggerEntity);
            triggerState.IsTriggered = true;
            EntityManager.SetComponentData(triggerEntity, triggerState);
        }
    }
}