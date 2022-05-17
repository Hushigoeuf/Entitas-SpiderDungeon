using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class CoconLifeRequestConfigSystem : ReactiveSystem<ConfigEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly FlightData _flightData;
        private readonly ContentSettingsObject _contentSettings;
        private readonly IGroup<FlightEntity> _spiderGroup;
        private readonly List<FlightEntity> _spiderBuffer;

        private ConfigEntity _efficiencyEntity;
        private ConfigEntity _coconEntity;

        public CoconLifeRequestConfigSystem(Contexts contexts, Data data) : base(contexts.config)
        {
            _contexts = contexts;
            _flightData = data.FlightData;
            _contentSettings = data.ContentData;

            _spiderGroup = contexts.flight.GetGroup(FlightMatcher.Spider);
            _spiderBuffer = new List<FlightEntity>();
        }

        public void Initialize()
        {
            foreach (var e in _contexts.config.GetEntities(ConfigMatcher.ContentObject))
            {
                if (_efficiencyEntity == null && e.hasBonusBehaviourType &&
                    e.bonusBehaviourType.Value == BonusBehaviourTypes.Efficiency)
                {
                    _efficiencyEntity = e;
                    _efficiencyEntity.OnDestroyEntity += _OnDestroyEfficiencyEntity;
                }

                if (_coconEntity == null && e.hasItemBehaviourType &&
                    e.itemBehaviourType.Value != ItemBehaviourTypes.Resurrection)
                {
                    _coconEntity = e;
                    _coconEntity.OnDestroyEntity += _OnDestroyCoconEntity;
                }

                if (_efficiencyEntity != null && _coconEntity != null) break;
            }
        }

        private void _OnDestroyEfficiencyEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= _OnDestroyEfficiencyEntity;
            _efficiencyEntity = null;
        }

        private void _OnDestroyCoconEntity(IEntity entity)
        {
            entity.OnDestroyEntity -= _OnDestroyCoconEntity;
            _coconEntity = null;
        }

        protected override ICollector<ConfigEntity> GetTrigger(IContext<ConfigEntity> context)
        {
            return context.CreateCollector(ConfigMatcher.CoconLifeRequest);
        }

        protected override bool Filter(ConfigEntity entity)
        {
            return true;
        }

        protected override void Execute(List<ConfigEntity> entities)
        {
            if (entities.Count == 0) return;
            //if (_contexts.config.mainConfigEntity.lifeCountInStorage.Value >= _flightData.MaxLifeInStorage) return;
            if (_contexts.config.mainConfigEntity.isGameOver) return;
            
            var level = 1;
            if (_coconEntity != null) level = _coconEntity.level.FullLevel;
            else if (_efficiencyEntity != null) level += _contentSettings.EfficiencyIncreaseLevelSize;
            var increaseValue = _contentSettings.CoconDropLife.GetCount(level);

            if (_contexts.config.mainConfigEntity.lifeCountInStorage.Value + increaseValue >
                _flightData.MaxLifeInStorage)
                increaseValue = _flightData.MaxLifeInStorage -
                                _contexts.config.mainConfigEntity.lifeCountInStorage.Value;

            _contexts.config.mainConfigEntity.ReplaceLifeCountInStorage(
                _contexts.config.mainConfigEntity.lifeCountInStorage.Value + increaseValue);

            _spiderGroup.GetEntities(_spiderBuffer);
            if (_spiderBuffer.Count >= _flightData.MaxSizeInFlight) return;

            for (var i = 0; i < _flightData.MaxSizeInFlight - _spiderBuffer.Count; i++)
            {
                if (_contexts.config.mainConfigEntity.lifeCountInStorage.Value < i + 1) break;
                var e = _contexts.flight.CreateEntity();
                {
                    e.isResurrection = true;
                }
            }
        }
    }
}