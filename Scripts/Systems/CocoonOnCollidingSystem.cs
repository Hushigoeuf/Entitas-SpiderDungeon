using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public class CocoonOnCollidingSystem : ReactiveSystem<ConfigEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly FlightSettings _flightSettings;
        private readonly ItemSettings _itemSettings;
        private readonly IGroup<FlightEntity> _characterGroup;
        private readonly List<FlightEntity> _characterBuffer;

        private ConfigEntity _configEntity;
        private ConfigEntity _cocoonEntity;

        public CocoonOnCollidingSystem(Contexts contexts, Settings settings) : base(contexts.config)
        {
            _contexts = contexts;
            _flightSettings = settings.FlightSettings;
            _itemSettings = settings.ItemSettings;
            _characterGroup = contexts.flight.GetGroup(FlightMatcher.Character);
            _characterBuffer = new List<FlightEntity>();
        }

        public void Initialize()
        {
            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.Item))
            {
                if (_configEntity == null && e.hasBonusItemType &&
                    e.bonusItemType.Value == BonusItemTypes.Efficiency)
                {
                    _configEntity = e;
                    _configEntity.OnDestroyEntity += OnDestroyBonusItemEntity;
                }

                if (_cocoonEntity == null && e.hasInventoryItemType &&
                    e.inventoryItemType.Value != InventoryItemTypes.Cocoon)
                {
                    _cocoonEntity = e;
                    _cocoonEntity.OnDestroyEntity += OnDestroyInventoryItemEntity;
                }

                if (_configEntity != null && _cocoonEntity != null) break;
            }
        }

        private void OnDestroyBonusItemEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= OnDestroyBonusItemEntity;
            _configEntity = null;
        }

        private void OnDestroyInventoryItemEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= OnDestroyInventoryItemEntity;
            _cocoonEntity = null;
        }

        protected override ICollector<ConfigEntity> GetTrigger(IContext<ConfigEntity> context) =>
            context.CreateCollector(ConfigMatcher.CocoonLifeRequest);

        protected override bool Filter(ConfigEntity entity) => true;

        protected override void Execute(List<ConfigEntity> entities)
        {
            if (entities.Count == 0) return;
            if (_contexts.config.mainConfigEntity.isGameOver) return;

            ReplaceLifeCountInStorage(GetIncreaseValue(GetLevel()));

            _characterGroup.GetEntities(_characterBuffer);
            if (_characterBuffer.Count >= _flightSettings.MaxSizeInFlight) return;

            for (var i = 0; i < _flightSettings.MaxSizeInFlight - _characterBuffer.Count; i++)
            {
                if (_contexts.config.mainConfigEntity.lifeCountInStorage.Value < i + 1) break;
                var e = _contexts.flight.CreateEntity();
                {
                    e.isResurrection = true;
                }
            }
        }

        private int GetLevel()
        {
            var level = 1;
            if (_cocoonEntity != null) level = _cocoonEntity.level.FullLevel;
            else if (_configEntity != null) level += _itemSettings.EfficiencyIncreaseLevelSize;
            return level;
        }

        private int GetIncreaseValue(int level)
        {
            var increaseValue = _itemSettings.CocoonDropLife.GetCount(level);
            if (_contexts.config.mainConfigEntity.lifeCountInStorage.Value + increaseValue >
                _flightSettings.MaxLifeInStorage)
                increaseValue = _flightSettings.MaxLifeInStorage -
                                _contexts.config.mainConfigEntity.lifeCountInStorage.Value;
            return increaseValue;
        }

        private void ReplaceLifeCountInStorage(int increaseValue)
        {
            _contexts.config.mainConfigEntity.ReplaceLifeCountInStorage(
                _contexts.config.mainConfigEntity.lifeCountInStorage.Value + increaseValue);
        }
    }
}