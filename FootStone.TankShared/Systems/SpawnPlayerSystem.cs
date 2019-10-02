﻿using Unity.Entities;
using Unity.Physics.Authoring;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    // [UpdateInGroup(typeof(InitializationSystemGroup))]
    [DisableAutoCreation]
    public class SpawnPlayerSystem : ComponentSystem
    {
        private Entity rocket;

        protected override void OnCreate()
        {
            var rocketPrefab = Resources.Load("Prefabs/Rocket") as GameObject;
            rocket = GameObjectConversionUtility.ConvertGameObjectHierarchy(rocketPrefab, World.Active);
        }

        protected override void OnUpdate()
        {

            Entities.ForEach(
               (Entity entity,ref LocalToWorld gunTransform, ref Rotation gunRotation, ref SpawnPlayer spawn) =>
               {                
                   if (spawn.spawned)
                   {
                       var buffer = EntityManager.GetBuffer<PlayerId>(entity);
                       foreach (var playerId in buffer)
                       {
                           //创建Player
                           var e = PostUpdateCommands.Instantiate(spawn.entity);
                           Translation position = new Translation() { Value = gunTransform.Position };
                           Rotation rotation = new Rotation() { Value = gunRotation.Value };

                           PostUpdateCommands.SetComponent(e, position);
                           PostUpdateCommands.SetComponent(e, rotation);
                           PostUpdateCommands.AddComponent(e, new Player() { playerId = playerId.playerId, id = e.Index });
                           PostUpdateCommands.AddComponent(e, new Attack() { Power = 10000 });
                           PostUpdateCommands.AddComponent(e, new Damage());
                           PostUpdateCommands.AddComponent(e, new Health() { Value = 30 });
                           PostUpdateCommands.AddComponent(e, new Score() { ScoreValue = 0, MaxScoreValue = 0 });
                           PostUpdateCommands.AddComponent(e, new UpdateUI());

                           PostUpdateCommands.AddComponent(e, new FireRocket()
                           {
                               Rocket = rocket,
                               FireCooldown = 0.1f,
                               RocketTimer = 0,
                           });
                           PostUpdateCommands.AddComponent(e, new MovePosition()
                           {
                               Speed = 5,
                           });

                           PostUpdateCommands.AddComponent(e, new PlayerCommand()
                           {
                               renderTick = 0,
                               targetPos = Vector3.zero

                           });
                       }

                       //移除SpawnPlayer
                       buffer.Clear();
                   }

                 //  spawn.players.Dispose();
                 //  PostUpdateCommands.RemoveComponent(entity,typeof(SpawnPlayer));
               });
        }
    }
}
