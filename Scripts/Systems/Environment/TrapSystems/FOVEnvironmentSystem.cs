using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class FOVEnvironmentSystem : IExecuteSystem, ICleanupSystem
    {
        private readonly ICameraService _cameraService;
        private readonly IGroup<EnvironmentEntity> _entities;
        private readonly List<EnvironmentEntity> _buffer;
        
        public FOVEnvironmentSystem(Contexts contexts, Services services)
        {
            _cameraService = services.CameraService;
            _entities = contexts.environment.GetGroup(EnvironmentMatcher.FOV);
            _buffer = new List<EnvironmentEntity>();
        }

        public void Execute()
        {
            _entities.GetEntities(_buffer);
            for (var i = 0; i < _buffer.Count; i++)
            {
                if (!_buffer[i].hasTransform) continue;
                var topPointPosition = GameSettings.GetTopTransformPoint(_cameraService.Container.position.y);
                if (_buffer[i].transform.Value.position.y > topPointPosition) continue;
                _buffer[i].isFOVEnabled = true;
                if (_buffer[i].hasEntityEnvironment) _buffer[i].entityEnvironment.Value.isFOVEnabled = true;
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