﻿using FootStone.ECS;
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
                    in ThrowPredictState throwState,
                    in SlicePredictedState slicePredictedState,
                    in WashPredictedState washPredictedState,
                    in SlotPredictedState slotState) =>
                {

                    if (HasSingleton<Server>())
                    {
                        SetInterpolateData(ref interpolateData,entity, transformPredictData,
                            slicePredictedState, washPredictedState, slotState, throwState);

                        SetInterpolateSqrMagnitude(ref interpolateData, velocityPredictData);
                    }
                    else
                    {
                        if (EntityManager.HasComponent<LocalCharacter>(entity))
                        {
                            SetInterpolateData(ref interpolateData,entity, transformPredictData,
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
            Entity entity,
            TransformPredictedState transformPredictData,
            SlicePredictedState slicePredictedState,
            WashPredictedState washPredictedState,
            SlotPredictedState slotState,
            ThrowPredictState throwState)
        {
            var replicatedEntityData = GetComponent<ReplicatedEntityData>(entity);
            interpolateData.MaterialId = replicatedEntityData.PredictingPlayerId;
         //   FSLog.Info($"replicatedEntityData.PredictingPlayerId:{replicatedEntityData.PredictingPlayerId}");
            interpolateData.Position = transformPredictData.Position;
            interpolateData.Rotation = transformPredictData.Rotation;
            interpolateData.IsTake = slotState.FilledIn != Entity.Null;
            interpolateData.IsSlice = slicePredictedState.IsSlicing;
            interpolateData.IsClean = washPredictedState.IsWashing;
            interpolateData.IsThrow = throwState.IsThrowed;

        }
    }
}
