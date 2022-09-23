using System.Collections.Generic;
using Entitas;
using UnityEngine;

namespace GameEngine
{
    public class SonarItemSystem : IInitializeSystem, IExecuteSystem
    {
        private readonly Contexts _contexts;
        private readonly ItemSettings _settings;
        private readonly Transform _camera;

        private bool _enabled;
        private IGroup<ConfigEntity> _sonarRequestGroup;
        private List<ConfigEntity> _sonarRequestList;

        public SonarItemSystem(Contexts contexts, Settings settings, Transform camera)
        {
            _contexts = contexts;
            _settings = settings.ItemSettings;
            _camera = camera;
        }

        public void Initialize()
        {
            foreach (var e in _contexts.config.GetEntities(
                         ConfigMatcher.AllOf(ConfigMatcher.Item, ConfigMatcher.InventoryItemType)))
            {
                if (e.inventoryItemType.Value != InventoryItemTypes.Sonar) continue;

                _enabled = true;
                e.OnDestroyEntity += OnDestroyItemEntity;
                break;
            }

            if (!_enabled) return;

            _sonarRequestGroup = _contexts.config.GetGroup(ConfigMatcher.SonarRequest);
            _sonarRequestList = new List<ConfigEntity>();
        }

        private void OnDestroyItemEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= OnDestroyItemEntity;

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

            var topPointPosition = ScreenSettings.GetTopScreenPoint(_camera.position.y);
            for (var i = 0; i < _sonarRequestList.Count; i++)
            {
                if (_sonarRequestList[i].position.Value.y > topPointPosition + _settings.SonarOffset) continue;
                _sonarRequestList[i].isEnabled = true;
                _sonarRequestList[i].isCleanup = true;
            }
        }
    }
}