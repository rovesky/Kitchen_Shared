using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.ECS
{
    public class CheckVisibleBehaviour : MonoBehaviour, IReceiveEntity
    {
        private Renderer _renderer;
        public Entity entity = Entity.Null;
        private bool isActive;


        public void SetReceivedEntity(Entity entity)
        {
            // Debug.Log($"CheckVisibleBehaviour SetReceivedEntity：{entity}");
            this.entity = entity;
        }

        private void OnEnable()
        {
            //  Debug.Log($"CheckVisibleBehaviour OnEnable");
        }

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>(); // 获得模型渲染组件
            // Debug.Log($"CheckVisibleBehaviour start begin{_renderer}");
            if (_renderer == null)
            {
                var rs = GetComponentsInChildren<MeshRenderer>();
                foreach (var render in rs)
                    if (!render.name.StartsWith("col"))
                    {
                        _renderer = render;
                        break;
                    }
            }

            // Debug.Log($"CheckVisibleBehaviour start end{_renderer}");
        }

        private void OnBecameVisible() // 当模型进入屏幕
        {
            //    Debug.Log($"CheckVisibleBehaviour OnBecameVisible");
            isActive = true;
        }

        public bool InVisible()
        {
            return isActive && !_renderer.isVisible && entity != Entity.Null;
        }
    }
}