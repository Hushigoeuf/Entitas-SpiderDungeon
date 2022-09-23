using Entitas;

namespace GameEngine
{
    public class LifeItemSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly FlightSettings _flightSettings;
        private readonly ItemSettings _itemSettings;

        public LifeItemSystem(Contexts contexts, Settings settings)
        {
            _contexts = contexts;
            _flightSettings = settings.FlightSettings;
            _itemSettings = settings.ItemSettings;
        }

        public void Initialize()
        {
            if (!_contexts.config.isMainConfig) return;
            if (!_contexts.config.mainConfigEntity.hasLifeCountInStorage) return;

            foreach (var e in _contexts.config.GetEntities(
                         ConfigMatcher.AllOf(ConfigMatcher.Item, ConfigMatcher.InventoryItemType)))
            {
                if (e.inventoryItemType.Value != InventoryItemTypes.AdditionalLife) continue;
                if (_contexts.config.mainConfigEntity.lifeCountInStorage.Value >=
                    _flightSettings.MaxLifeInStorage) break;

                int increaseValue = _itemSettings.LifeDropCount.GetCount(e.level.FullLevel);
                if (_contexts.config.mainConfigEntity.lifeCountInStorage.Value + increaseValue >=
                    _flightSettings.MaxLifeInStorage)
                    increaseValue = _flightSettings.MaxLifeInStorage -
                                    _contexts.config.mainConfigEntity.lifeCountInStorage.Value;
                _contexts.config.mainConfigEntity.ReplaceLifeCountInStorage(
                    _contexts.config.mainConfigEntity.lifeCountInStorage.Value + increaseValue);

                break;
            }
        }
    }
}