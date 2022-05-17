using Entitas;

namespace GameEngine
{
    public sealed class LifeItemConfigSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly FlightData _flightData;
        private readonly ContentSettingsObject _contentSettings;

        public LifeItemConfigSystem(Contexts contexts, Data data)
        {
            _contexts = contexts;
            _flightData = data.FlightData;
            _contentSettings = data.ContentData;
        }

        public void Initialize()
        {
            if (!_contexts.config.isMainConfig) return;
            if (!_contexts.config.mainConfigEntity.hasLifeCountInStorage) return;

            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.AllOf(ConfigMatcher.ContentObject,
                ConfigMatcher.ItemBehaviourType)))
            {
                if (e.itemBehaviourType.Value != ItemBehaviourTypes.Life) continue;
                if (_contexts.config.mainConfigEntity.lifeCountInStorage.Value >= _flightData.MaxLifeInStorage) break;

                var increaseValue = _contentSettings.LifeDropCount.GetCount(e.level.FullLevel);
                if (_contexts.config.mainConfigEntity.lifeCountInStorage.Value + increaseValue >= _flightData.MaxLifeInStorage)
                    increaseValue = _flightData.MaxLifeInStorage - _contexts.config.mainConfigEntity.lifeCountInStorage.Value;
                _contexts.config.mainConfigEntity.ReplaceLifeCountInStorage(
                    _contexts.config.mainConfigEntity.lifeCountInStorage.Value + increaseValue);
                break;
            }
        }
    }
}