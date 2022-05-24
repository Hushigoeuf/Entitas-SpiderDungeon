using Entitas;

namespace GameEngine
{
    /// <summary>
    /// Уменьшает скорость движения если стоит предмет на удачу.
    /// </summary>
    public sealed class LuckSpeedSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly ItemSettings _contentSettings;
        private readonly FlightSettings _flightSettings;

        private ConfigEntity _luckEntity;
        private FlightEntity _movementEntity;

        public LuckSpeedSystem(Contexts contexts, Settings settings)
        {
            _contexts = contexts;
            _contentSettings = settings.ItemSettings;
            _flightSettings = settings.FlightSettings;
        }

        public void Initialize()
        {
            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.AllOf(ConfigMatcher.Item,
                         ConfigMatcher.InventoryItemType)))
            {
                if (e.inventoryItemType.Value != InventoryItemTypes.Luck) continue;

                _luckEntity = e;
                _luckEntity.OnDestroyEntity += OnDestroyItemEntity;
                break;
            }

            if (_luckEntity == null) return;

            _movementEntity = _contexts.flight.movementEntity;
            _movementEntity.ReplaceSpeed(_flightSettings.Speed -
                                         _contentSettings.LuckDropSpeed.GetTime(_luckEntity.level.FullLevel));
        }

        private void OnDestroyItemEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= OnDestroyItemEntity;

            _luckEntity = null;
            _movementEntity.ReplaceSpeed(_flightSettings.Speed);
            _movementEntity = null;
        }
    }
}