﻿using FootStone.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

namespace FootStone.Kitchen
{

    [DisableAutoCreation]
    public class MoveTargetSystem : ComponentSystem
    {
        public EntityQuery PlayerGroup;

        protected override void OnCreate()
        {
            PlayerGroup = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                   typeof(Player),
                }
            });
        }

        protected override void OnUpdate()
        {
            var playerEntities = PlayerGroup.ToEntityArray(Allocator.Persistent);

            if (playerEntities.Length == 0)
            {
                playerEntities.Dispose();
                return;
            }

            Entities.ForEach((ref Translation position, ref Rotation rotation, ref MoveTarget move) =>
            {
                var tickDuration = GetSingleton<WorldTime>().GameTick.TickDuration;

                var target = playerEntities[0];

                var targetPos = EntityManager.GetComponentData<Translation>(target);
                var targetRotation = EntityManager.GetComponentData<Rotation>(target);

                Vector3 value = Vector3.MoveTowards(position.Value, targetPos.Value, move.Speed * tickDuration);

                position = new Translation()
                {
                    Value = value
                };

                Vector3 relativePos = position.Value - targetPos.Value;

               // if(relativePos != Vector3.zero)

                rotation = new Rotation()
                {
                    Value = Quaternion.LookRotation(relativePos)
                };
            });
            playerEntities.Dispose();
        }
    }
}
