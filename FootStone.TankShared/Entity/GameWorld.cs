using UnityEngine;
using System.Collections.Generic;
using Object = UnityEngine.Object;

using Unity.Entities;
using UnityEditor;
using UnityEngine.Profiling;
using System.Diagnostics;

namespace FootStone.ECS
{

    public static class WorldExtension
    {
        public static T GetOrCreateSystemE<T>(this World world) where T : FSComponentSystem
        {
           // FSLog.Info($"GetOrCreateSystem: {typeof(T).Name}");
            var system =  world.GetOrCreateSystem<T>();
            system.GameWorld = GameWorld.Active;
            return system;
        }
    }

   

    public class GameWorld
    {

        // TODO (petera) this is kind of ugly. But very useful to look at worlds from outside for stats purposes...
       // public static List<GameWorld> s_Worlds = new List<GameWorld>();

        public static GameWorld Active { get; set; }
        public double FrameTime { get => frameTime; set => frameTime = value; }
        public Stopwatch Clock { get => clock; set => clock = value; }

        private long stopwatchFrequency;
        private Stopwatch clock;
        private double frameTime;

   
        public GameWorld(string name = "world")
        {
            GameDebug.Log("GameWorld " + name + " initializing");

            stopwatchFrequency = Stopwatch.Frequency;
            Clock = new Stopwatch();
            Clock.Start();
        }

        public void Update()
        {
            FrameTime = (double)Clock.ElapsedTicks / stopwatchFrequency;
        }

        //public void Shutdown()
        //{
        // //   GameDebug.Log("GameWorld " + ecsWorld.Name + " shutting down");

        // //   foreach (var entity in m_dynamicEntities)
        // //   {
        // //       if (m_DespawnRequests.Contains(entity))
        // //           continue;

        // //       //#if UNITY_EDITOR
        // //       if (entity == null)
        // //           continue;

        // //       var gameObjectEntity = entity.GetComponent<GameObjectEntity>();
        // //       if (gameObjectEntity != null && !m_EntityManager.Exists(gameObjectEntity.Entity))
        // //           continue;
        // //       //#endif            

        // //       RequestDespawn(entity);
        // //   }
        // //   ProcessDespawns();

        // ////   s_Worlds.Remove(this);

        // //   GameObject.Destroy(m_sceneRoot);
        //}

    }

}