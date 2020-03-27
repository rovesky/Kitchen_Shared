using FootStone.ECS;
using Unity.Entities;
using Unity.Mathematics;

namespace FootStone.Kitchen
{
    [DisableAutoCreation]
    public class TestSystem : SystemBase
    {
        private Random random;
        private float3 lastVelocity;

        protected override void OnCreate()
        {
            random = new Random(1);
        }

        protected override void OnUpdate()
        {
            Entities
                .WithAll<Character>()
                .WithStructuralChanges().ForEach((Entity entity,
                    ref VelocityPredictedState velocityPredictedState) =>
                {
                    if (random.NextInt(0, 100) > 95)
                    {
                        velocityPredictedState.Linear = new float3(random.NextFloat(-4.0f, 4.0f), 0, 0);
                        lastVelocity = velocityPredictedState.Linear;
                    }
                    else
                        velocityPredictedState.Linear = lastVelocity;
                   
                    FSLog.Info($" velocityPredictedState.Linear:{velocityPredictedState.Linear}");
                }).Run();
        }
    }

}