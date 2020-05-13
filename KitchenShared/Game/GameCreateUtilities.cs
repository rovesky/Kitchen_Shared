using FootStone.ECS;
using Unity.Entities;

namespace FootStone.Kitchen
{
    public static class GameCreateUtilities
    {

        public static Entity CreateGame(EntityManager entityManager)
        {
            var e = entityManager.CreateEntity(typeof(ReplicatedEntityData),
                typeof(Countdown), typeof(Score), typeof(GameEntity), typeof(GameStateComponent));
            entityManager.SetComponentData(e, new ReplicatedEntityData()
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            entityManager.SetComponentData(e, new GameEntity()
            { 
                Type = EntityType.Game
            }); 
        
            var countDown = new Countdown();
            countDown.SetValue(0);
            entityManager.SetComponentData(e,countDown);

            entityManager.SetComponentData(e, new Score()
            {
                Value =  0
            });

            entityManager.SetComponentData(e, new GameStateComponent()
            {
                State =  GameState.Ending
            });
            return e;
        }


        public static Entity CreateMenuItem(EntityManager entityManager)
        {
            var e = entityManager.CreateEntity(typeof(ReplicatedEntityData), typeof(MenuItem),typeof(GameEntity));
            entityManager.SetComponentData(e, new ReplicatedEntityData()
            {
                Id = -1,
                PredictingPlayerId = -1
            });

            entityManager.SetComponentData(e, new GameEntity()
            { 
                Type = EntityType.Menu
            });

            entityManager.SetComponentData(e, new MenuItem());

            entityManager.AddComponentData(e, new DespawnPredictedState()
            {
                IsDespawn = false,
                Tick = 0
            });
            return e;
        }

    }

}