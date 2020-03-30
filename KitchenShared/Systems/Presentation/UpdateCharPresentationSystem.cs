﻿using FootStone.ECS;
using Unity.Entities;

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
                    in ReplicatedEntityData replicatedEntityData) =>
                {
                    interpolateData.Position = transformPredictData.Position;
                    interpolateData.Rotation = transformPredictData.Rotation;
                    //var dir = Vector3.SqrMagnitude(velocityPredictData.Linear) < 0.001f
                    //    ? Vector3.zero: (Vector3) math.normalize(velocityPredictData.Linear);
                    //if(!dir.Equals(Vector3.zero))
                    //    FSLog.Info($"UpdateCharPresentationSystem,entity:{entity},velocityPredictData:{velocityPredictData.Linear}");
                    //interpolateData.SqrMagnitude = new Vector2(dir.x, dir.z).sqrMagnitude;

                    interpolateData.SqrMagnitude = velocityPredictData.SqrMagnitude;
                    interpolateData.MaterialId = replicatedEntityData.Id % 4;

                    //setup trigger entity 
                    if (triggerPredictedData.TriggeredEntity == Entity.Null)
                        return;

                    var triggerEntity = triggerPredictedData.TriggeredEntity;
                    var triggerState = EntityManager.GetComponentData<TriggeredState>(triggerEntity);
                    triggerState.IsTriggered = true;
                    EntityManager.SetComponentData(triggerEntity, triggerState);

                }).Run();
        }
    }
}