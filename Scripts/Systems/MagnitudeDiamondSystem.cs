using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class MagnitudeDiamondSystem : IExecuteSystem, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IGroup<EnvironmentEntity> _group;
        private readonly List<EnvironmentEntity> _buffer;

        private ConfigEntity _configEntity;

        public MagnitudeDiamondSystem(Contexts contexts)
        {
            _contexts = contexts;
            _group = contexts.environment.GetGroup(EnvironmentMatcher.MagnitudeDiamond);
            _buffer = new List<EnvironmentEntity>();
        }

        public void Initialize()
        {
            _configEntity = _contexts.config.mainConfigEntity;
        }

        public void Execute()
        {
            if (_configEntity.isGameOver) return;

            _group.GetEntities(_buffer);
            for (var i = 0; i < _buffer.Count; i++)
            {
                if (!_buffer[i].hasTransform || !_buffer[i].hasTarget || !_buffer[i].hasSpeed)
                {
                    _buffer[i].isCleanup = true;
                    continue;
                }

                var position = _buffer[i].transform.Value.position;
                var resultPosition = Vector3.Lerp(_buffer[i].transform.Value.position,
                    _buffer[i].target.Value.position, _buffer[i].speed.Value * Time.deltaTime);
                position.x = resultPosition.x;
                position.y = resultPosition.y;
                _buffer[i].transform.Value.position = position;
            }
        }
    }
}