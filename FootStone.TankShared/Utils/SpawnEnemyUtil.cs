using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public static class SpawnEnemyUtil
    {
        public static Entity SpwanEnemy(EntityManager PostUpdateCommands, GameObject prefab,EnemyType type,
            float3 pos,Entity rocket)
        {
            var go = Object.Instantiate(prefab);
            var e = go.GetComponent<EntityTracker>().EntityToTrack;
            // var e = PostUpdateCommands.Instantiate(spawn.entity);

            Translation position = new Translation() { Value = pos };
            Rotation rotation = new Rotation() { Value = Quaternion.identity };

            PostUpdateCommands.SetComponentData(e, position);
            PostUpdateCommands.SetComponentData(e, rotation);

            PostUpdateCommands.AddComponentData(e, new Enemy() { type = type, id = e.Index });
            PostUpdateCommands.AddComponentData(e, new Damage());
            PostUpdateCommands.AddComponentData(e, new Attack() { Power = 1 });
      

            if (type == EnemyType.Normal)
            {
                PostUpdateCommands.AddComponentData(e, new Health() { Value = 100 });
                PostUpdateCommands.AddComponentData(e, new MoveTranslation() { Speed = 1f, Direction = Direction.Down });
                PostUpdateCommands.AddComponentData(e, new MoveSin());
            }
            else if (type == EnemyType.Super)
            {
                PostUpdateCommands.AddComponentData(e, new Health() { Value = 500 });
                PostUpdateCommands.AddComponentData(e, new MoveTranslation() { Speed = 0.5f, Direction = Direction.Down });
                PostUpdateCommands.AddComponentData(e, new FireRocket()
                {
                    Rocket = rocket,
                    FireCooldown = 2f,
                    RocketTimer = 4f,
                });
            }
            return e;
        }

        public static Entity SpwanPlayer(EntityManager PostUpdateCommands,int playerId, Entity prefab, float3 pos, Entity rocket)
        {
            //创建Player
            var e = PostUpdateCommands.Instantiate(prefab);
            Translation position = new Translation() { Value = pos };
            Rotation rotation = new Rotation() { Value = Quaternion.identity};
      
            PostUpdateCommands.SetComponentData(e, position);
         //   PostUpdateCommands.SetComponentData(e, rotation);
            PostUpdateCommands.AddComponentData(e, new Player() { playerId = playerId, id = e.Index });
            PostUpdateCommands.AddComponentData(e, new Attack() { Power = 10000 });
            PostUpdateCommands.AddComponentData(e, new Damage());
            PostUpdateCommands.AddComponentData(e, new Health() { Value = 30 });
            PostUpdateCommands.AddComponentData(e, new Score() { ScoreValue = 0, MaxScoreValue = 0 });
            PostUpdateCommands.AddComponentData(e, new UpdateUI());

            PostUpdateCommands.AddComponentData(e, new FireRocket()
            {
                Rocket = rocket,
                FireCooldown = 0.1f,
                RocketTimer = 0,
            });
            PostUpdateCommands.AddComponentData(e, new MovePosition()
            {
                Speed = 5,
            });

            //PostUpdateCommands.AddComponentData(e, new PlayerCommand()
            //{
            //    renderTick = 0,
            //    targetPos = Vector3.zero

            //});
            return e;
        }
    }
}
