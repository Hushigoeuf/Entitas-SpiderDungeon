using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    /// <summary>
    /// Вычисляет позицию объектов и определяет, находится ли объект в видимое зоне.
    /// </summary>
    public sealed class FOVSystem : IExecuteSystem, ICleanupSystem
    {
        private readonly IGroup<EnvironmentEntity> _entities;
        private readonly List<EnvironmentEntity> _buffer;
        private readonly Transform _camera;

        public FOVSystem(Contexts contexts, Transform camera)
        {
            _entities = contexts.environment.GetGroup(EnvironmentMatcher.FOV);
            _buffer = new List<EnvironmentEntity>();
            _camera = camera;
        }

        public void Execute()
        {
            var topPointPosition = ScreenSettings.GetTopScreenPoint(_camera.position.y);

            _entities.GetEntities(_buffer);
            for (var i = 0; i < _buffer.Count; i++)
            {
                if (!_buffer[i].hasTransform) continue;
                if (_buffer[i].transform.Value.position.y > topPointPosition) continue;

                _buffer[i].isFOVEnabled = true;
                if (_buffer[i].hasEntityEnvironment)
                    _buffer[i].entityEnvironment.Value.isFOVEnabled = true;
            }
        }

        public void Cleanup()
        {
            foreach (var e in _buffer)
                if (e.isFOVEnabled)
                    e.Destroy();
        }
    }
}