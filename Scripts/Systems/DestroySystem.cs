using System.Collections.Generic;
using Entitas;
using Entitas.VisualDebugging.Unity;

namespace GameEngine
{
    public class DestroySystem : IExecuteSystem
    {
        private readonly IGroup<EnvironmentEntity> _group;
        private readonly IPoolService _poolService;
        private readonly List<EnvironmentEntity> _buffer = new List<EnvironmentEntity>();

        public DestroySystem(Contexts contexts, Services services)
        {
            _group = contexts.environment.GetGroup(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.Destroy,
                EnvironmentMatcher.Transform,
                EnvironmentMatcher.Target));
            _poolService = services.Pool;
        }

        public void Execute()
        {
            _group.GetEntities(_buffer);
            if (_buffer.Count == 0) return;
            for (var i = 0; i < _buffer.Count; i++)
            {
                if (_buffer[i].target.Value != null)
                {
                    float offset = _buffer[i].hasOffset ? _buffer[i].offset.Value : 0;
                    if (_buffer[i].target.Value.position.y > _buffer[i].transform.Value.position.y - offset) continue;
                    if (_buffer[i].hasPool)
                    {
                        if (_poolService.Contains(_buffer[i].pool.Value, _buffer[i].target.Value))
                            _poolService.Despawn(_buffer[i].pool.Value, _buffer[i].target.Value);
                    }
                    else
                    {
                        _buffer[i].target.Value.gameObject.DestroyGameObject();
                    }
                }

                _buffer[i].isCleanup = true;
            }
        }
    }
}