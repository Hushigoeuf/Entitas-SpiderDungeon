using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class RotationEnvironmentSystem : IExecuteSystem
    {
        private readonly ITimeService _timeService;
        private readonly IGroup<EnvironmentEntity> _entities;
        private readonly List<EnvironmentEntity> _buffer;

        public RotationEnvironmentSystem(Contexts contexts, Services services)
        {
            _timeService = services.TimeService;
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
                if (_buffer[i].direction.Value.x == -1 || _buffer[i].direction.Value.x == 1)
                    eulerAngles.x += _buffer[i].speed.Value * _buffer[i].direction.Value.x * _timeService.DeltaTime;
                if (_buffer[i].direction.Value.y == -1 || _buffer[i].direction.Value.y == 1)
                    eulerAngles.y += _buffer[i].speed.Value * _buffer[i].direction.Value.y * _timeService.DeltaTime;
                if (_buffer[i].direction.Value.z == -1 || _buffer[i].direction.Value.z == 1)
                    eulerAngles.z += _buffer[i].speed.Value * _buffer[i].direction.Value.z * _timeService.DeltaTime;
                _buffer[i].transform.Value.eulerAngles = eulerAngles;
            }
        }
    }
}