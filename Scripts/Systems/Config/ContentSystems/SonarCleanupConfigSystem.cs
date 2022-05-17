using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class SonarCleanupConfigSystem : IInitializeSystem, IExecuteSystem
    {
        private readonly Contexts _contexts;
        private readonly ICameraService _cameraService;
        private readonly float _offset;
        
        private bool _enabled;
        private IGroup<ConfigEntity> _sonarRequestGroup;
        private List<ConfigEntity> _sonarRequestList;

        public SonarCleanupConfigSystem(Contexts contexts, Services services, Data data)
        {
            _contexts = contexts;
            _cameraService = services.CameraService;
            _offset = data.ContentData.SonarOffset;
        }

        public void Initialize()
        {
            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.AllOf(ConfigMatcher.ContentObject,
                ConfigMatcher.ItemBehaviourType)))
            {
                if (e.itemBehaviourType.Value != ItemBehaviourTypes.Sonar) continue;
                _enabled = true;
                e.OnDestroyEntity += _OnDestroySonarEntity;
                break;
            }

            if (!_enabled) return;

            _sonarRequestGroup = _contexts.config.GetGroup(ConfigMatcher.SonarRequest);
            _sonarRequestList = new List<ConfigEntity>();
        }

        private void _OnDestroySonarEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= _OnDestroySonarEntity;
            _enabled = false;

            _sonarRequestGroup.GetEntities(_sonarRequestList);
            if (_sonarRequestList.Count == 0) return;
            for (var i = 0; i < _sonarRequestList.Count; i++)
                _sonarRequestList[i].isCleanup = true;
        }

        public void Execute()
        {
            if (!_enabled) return;
            _sonarRequestGroup.GetEntities(_sonarRequestList);
            if (_sonarRequestList.Count == 0) return;

            var topPointPosition = GameSettings.GetTopTransformPoint(_cameraService.Container.position.y);
            for (var i = 0; i < _sonarRequestList.Count; i++)
            {
                if (_sonarRequestList[i].position.Value.y > topPointPosition + _offset) continue;
                _sonarRequestList[i].isEnabled = true;
                _sonarRequestList[i].isCleanup = true;
            }
        }
    }
}