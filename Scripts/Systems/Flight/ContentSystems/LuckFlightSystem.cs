using Entitas;

namespace GameEngine
{
    public sealed class LuckFlightSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly ContentSettingsObject _contentSettings;
        private readonly FlightData _flightData;
        private ConfigEntity _luckEntity;
        private FlightEntity _movementEntity;

        public LuckFlightSystem(Contexts contexts, Data data)
        {
            _contexts = contexts;
            _contentSettings = data.ContentData;
            _flightData = data.FlightData;
        }

        public void Initialize()
        {
            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.AllOf(ConfigMatcher.ContentObject,
                ConfigMatcher.ItemBehaviourType)))
            {
                if (e.itemBehaviourType.Value != ItemBehaviourTypes.Luck) continue;
                _luckEntity = e;
                _luckEntity.OnDestroyEntity += _OnDestroyLuckEntity;
                break;
            }

            if (_luckEntity == null) return;

            _movementEntity = _contexts.flight.movementEntity;
            _movementEntity.ReplaceSpeed(_flightData.Speed -
                                         _contentSettings.LuckDropSpeed.GetTime(_luckEntity.level.FullLevel));
        }

        private void _OnDestroyLuckEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= _OnDestroyLuckEntity;
            _luckEntity = null;
            _movementEntity.ReplaceSpeed(_flightData.Speed);
            _movementEntity = null;
        }
    }
}