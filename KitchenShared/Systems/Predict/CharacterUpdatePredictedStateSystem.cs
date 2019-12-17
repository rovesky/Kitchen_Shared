using Unity.Entities;
using Unity.Transforms;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class CharacterUpdatePredictedStateSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((Entity entity,
                ref CharacterPredictedState predictedState,
                ref Translation translation,
                ref Rotation rotation) =>
            {
                translation.Value = predictedState.Position;
                rotation.Value = predictedState.Rotation;
            });
        }
    }
}