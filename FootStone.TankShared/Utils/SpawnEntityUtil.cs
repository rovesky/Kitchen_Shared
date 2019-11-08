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
    public static class SpawnEntityUtil
    {
        public static Entity SpwanEnemy(EntityManager em, GameObject prefab,EnemyType type,
            float3 pos,Entity rocket)
        {
            var go = Object.Instantiate(prefab);
            var e = go.GetComponent<EntityTracker>().EntityToTrack;
            // var e = PostUpdateCommands.Instantiate(spawn.entity);

            Translation position = new Translation() { Value = pos };
            Rotation rotation = new Rotation() { Value = Quaternion.identity };

            em.SetComponentData(e, position);
            em.SetComponentData(e, rotation);

            em.AddComponentData(e, new Enemy() { type = type, id = e.Index });
            em.AddComponentData(e, new Damage());
            em.AddComponentData(e, new Attack() { Power = 1 });
      

            if (type == EnemyType.Normal)
            {
                em.AddComponentData(e, new Health() { Value = 100 });
                em.AddComponentData(e, new MoveForward() { Speed = 1f});
                //     em.AddComponentData(e, new MoveTranslation() { Speed = 1f, Direction = Direction.Down });
                em.AddComponentData(e, new MoveSin());
            }
            else if (type == EnemyType.Super)
            {
                em.AddComponentData(e, new Health() { Value = 500 });
                em.AddComponentData(e, new MoveForward() { Speed = 0.5f });
                //   em.AddComponentData(e, new MoveTranslation() { Speed = 0.5f, Direction = Direction.Down });
                em.AddComponentData(e, new FireRocket()
                {
                    Rocket = rocket,
                    FireCooldown = 2f,
                    RocketTimer = 4f,
                });
            }
            return e;
        }

        public static Entity SpwanPlayer(EntityManager em,int playerId, Entity prefab, float3 pos, Entity rocket)
        {
            //创建Player
            var e = em.Instantiate(prefab);
            Translation position = new Translation() { Value = pos };
            Rotation rotation = new Rotation() { Value = Quaternion.identity};
      
            em.SetComponentData(e, position);

            em.AddComponentData(e, new Player() { playerId = playerId, id = e.Index });
            em.AddComponentData(e, new Attack() { Power = 10000 });
            em.AddComponentData(e, new Damage());
            em.AddComponentData(e, new Health() { Value = 30 });
            em.AddComponentData(e, new Score() { ScoreValue = 0, MaxScoreValue = 0 });
            em.AddComponentData(e, new UpdateUI());

            em.AddComponentData(e, new FireRocket()
            {
                Rocket = rocket,
                FireCooldown = 0.1f,
                RocketTimer = 0,
            });
            em.AddComponentData(e, new MovePosition()
            {
                Speed = 5,
            });

            em.AddComponentData(e, new MoveInput()
            {
                Speed = 6,
            });
            return e;
        }
    }
}
