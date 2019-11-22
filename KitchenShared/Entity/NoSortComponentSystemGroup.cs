using UnityEngine;
using System.Collections.Generic;


using Unity.Entities;
using UnityEditor;
using UnityEngine.Profiling;

namespace FootStone.ECS
{
    [DisableAutoCreation]
    public abstract class NoSortComponentSystemGroup : ComponentSystemGroup
    {
        public override void SortSystemUpdateList()
        {

        }

    }


    //public abstract class FSComponentSystem: ComponentSystem
    //{
    //    private GameWorld gameWorld;

    //    public GameWorld GameWorld { get => gameWorld; set => gameWorld = value; }
    //}

}