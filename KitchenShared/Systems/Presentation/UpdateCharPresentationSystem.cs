using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class UpdateCharTriggeredSystem : SystemBase
    {

        protected override void OnUpdate()
        {
            Entities.WithAll<ServerEntity>()
                .WithStructuralChanges()
                .ForEach((Entity entity,
                    in TriggerPredictedState triggerPredictedData
                ) =>
                {
                    if (triggerPredictedData.TriggeredEntity == Entity.Null)
                        return;
                    var triggerEntity = triggerPredictedData.TriggeredEntity;
                    var triggerState = EntityManager.GetComponentData<TriggeredState>(triggerEntity);
                  //  triggerState.IsTriggered = true;
                    triggerState.TriggerEntity = entity;
                    EntityManager.SetComponentData(triggerEntity, triggerState);

                }).Run();
        }
    }

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
                    in ThrowPredictState throwState,
                    in SlicePredictedState slicePredictedState,
                    in WashPredictedState washPredictedState,
                    in SlotPredictedState slotState) =>
                {

                    if (HasSingleton<Server>())
                    {
                        SetInterpolateData(ref interpolateData, transformPredictData,
                            slicePredictedState, washPredictedState, slotState, throwState);

                        SetInterpolateSqrMagnitude(ref interpolateData, velocityPredictData);
                    }
                    else
                    {
                        if (EntityManager.HasComponent<LocalCharacter>(entity))
                        {
                            SetInterpolateData(ref interpolateData, transformPredictData,
                                slicePredictedState, washPredictedState, slotState, throwState);

                            var dir = Vector3.SqrMagnitude(velocityPredictData.Linear) < 0.001f
                                ? Vector3.zero
                                : (Vector3) math.normalize(velocityPredictData.Linear);
                            interpolateData.Velocity = new Vector2(dir.x, dir.z).sqrMagnitude;

                        }
                        else
                        {
                            SetInterpolateSqrMagnitude(ref interpolateData, velocityPredictData);
                        }
                    }
                }).Run();
        }

        private void SetInterpolateSqrMagnitude(ref CharacterInterpolatedState interpolateData,
            VelocityPredictedState velocityPredictData)
        {
            if (velocityPredictData.SqrMagnitude > 0)
            {
                interpolateData.Velocity = velocityPredictData.SqrMagnitude;
            }
            else
            {
                var dir = Vector3.SqrMagnitude(velocityPredictData.Linear) < 0.001f
                    ? Vector3.zero
                    : (Vector3) math.normalize(velocityPredictData.Linear);
                interpolateData.Velocity = new Vector2(dir.x, dir.z).sqrMagnitude;
            }
        }

        private void SetInterpolateData(ref CharacterInterpolatedState interpolateData,
            TransformPredictedState transformPredictData,
            SlicePredictedState slicePredictedState,
            WashPredictedState washPredictedState,
            SlotPredictedState slotState,
            ThrowPredictState throwState)
        {
            interpolateData.Position = transformPredictData.Position;
            interpolateData.Rotation = transformPredictData.Rotation;
            interpolateData.IsTake = slotState.FilledIn != Entity.Null;
            interpolateData.IsSlice = slicePredictedState.IsSlicing;
            interpolateData.IsClean = washPredictedState.IsWashing;
            interpolateData.IsThrow = throwState.IsThrowed;

        }
    }
}
