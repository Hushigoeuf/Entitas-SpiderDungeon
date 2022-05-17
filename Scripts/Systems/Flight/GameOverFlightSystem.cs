using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class GameOverFlightSystem : ReactiveSystem<FlightEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;

        private readonly IGroup<FlightEntity> _spiderGroup;
        private readonly List<FlightEntity> _spiderBuffer;

        private ConfigEntity _mainConfigEntity;

        public GameOverFlightSystem(Contexts contexts) : base(contexts.flight)
        {
            _contexts = contexts;

            _spiderGroup = contexts.flight.GetGroup(FlightMatcher.Spider);
            _spiderBuffer = new List<FlightEntity>();
        }

        public void Initialize()
        {
            if (_contexts.config.isMainConfig)
                _mainConfigEntity = _contexts.config.mainConfigEntity;
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context)
        {
            return context.CreateCollector(FlightMatcher.Dead);
        }


        protected override bool Filter(FlightEntity entity)
        {
            return !_mainConfigEntity.isGameOver;
        }

        protected override void Execute(List<FlightEntity> entities)
        {
            if (entities.Count == 0) return;
            if (_mainConfigEntity.isGameOver) return;

            if (_mainConfigEntity.lifeCountInStorage.Value > 0) return;

            _spiderGroup.GetEntities(_spiderBuffer);
            var notDeadFound = false;
            for (var i = 0; i < _spiderBuffer.Count; i++)
            {
                if (_spiderBuffer[i].hasDead) continue;
                notDeadFound = true;
                break;
            }

            if (notDeadFound) return;

            _mainConfigEntity.isGameOver = true;
        }
    }
}