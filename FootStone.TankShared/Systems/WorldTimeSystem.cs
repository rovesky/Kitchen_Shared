using FootStone.ECS;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
 
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public class WorldTimeSystem : FSComponentSystem
    {
        private long stopwatchFrequency;
        private Stopwatch clock;
        private double frameTime;

        protected override void OnCreate()
        {
            base.OnCreate();

            stopwatchFrequency = Stopwatch.Frequency;
            clock = new Stopwatch();
            clock.Start();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            clock.Stop();
        }
        protected override void OnUpdate()
        {
            Game.frameTime = (double)clock.ElapsedTicks / stopwatchFrequency;
        }
    }
}