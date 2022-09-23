using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class RotationSystem : IExecuteSystem
    {
        private readonly IGroup<EnvironmentEntity> _entities;
        private readonly List<EnvironmentEntity> _buffer;

        public RotationSystem(Contexts contexts)
        {
            _entities = contexts.environment.GetGroup(EnvironmentMatcher.AllOf(
                EnvironmentMatcher.Rotation,
                EnvironmentMatcher.Transform,
                EnvironmentMatcher.Speed,
                EnvironmentMatcher.Direction));
            _buffer = new List<EnvironmentEntity>();
        }

        public void Execute()
        {
            _entities.GetEntities(_buffer);
            for (var i = 0; i < _buffer.Count; i++)
            {
                if (!_buffer[i].isEnabled) continue;
                var eulerAngles = _buffer[i].transform.Value.eulerAngles;
                float dt = Time.deltaTime;
                if (_buffer[i].direction.Value.x == -1 || _buffer[i].direction.Value.x == 1)
                    eulerAngles.x += _buffer[i].speed.Value * _buffer[i].direction.Value.x * dt;
                if (_buffer[i].direction.Value.y == -1 || _buffer[i].direction.Value.y == 1)
                    eulerAngles.y += _buffer[i].speed.Value * _buffer[i].direction.Value.y * dt;
                if (_buffer[i].direction.Value.z == -1 || _buffer[i].direction.Value.z == 1)
                    eulerAngles.z += _buffer[i].speed.Value * _buffer[i].direction.Value.z * dt;
                _buffer[i].transform.Value.eulerAngles = eulerAngles;
            }
        }
    }
}