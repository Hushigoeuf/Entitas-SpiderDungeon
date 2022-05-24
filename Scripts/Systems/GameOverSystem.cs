using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    /// <summary>
    /// Следит за жизнями персонажей и объявляет проигрыш если все погибнут.
    /// </summary>
    public sealed class GameOverSystem : ReactiveSystem<FlightEntity>, IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IGroup<FlightEntity> _characterGroup;
        private readonly List<FlightEntity> _characterBuffer;

        private ConfigEntity _mainConfigEntity;

        public GameOverSystem(Contexts contexts) : base(contexts.flight)
        {
            _contexts = contexts;
            _characterGroup = contexts.flight.GetGroup(FlightMatcher.Character);
            _characterBuffer = new List<FlightEntity>();
        }

        public void Initialize()
        {
            if (_contexts.config.isMainConfig)
                _mainConfigEntity = _contexts.config.mainConfigEntity;
        }

        protected override ICollector<FlightEntity> GetTrigger(IContext<FlightEntity> context) =>
            context.CreateCollector(FlightMatcher.Death);

        protected override bool Filter(FlightEntity entity) => !_mainConfigEntity.isGameOver;

        protected override void Execute(List<FlightEntity> entities)
        {
            if (entities.Count == 0) return;
            if (_mainConfigEntity.isGameOver) return;

            if (_mainConfigEntity.lifeCountInStorage.Value > 0) return;

            _characterGroup.GetEntities(_characterBuffer);
            var notDeathFound = false;
            for (var i = 0; i < _characterBuffer.Count; i++)
            {
                if (_characterBuffer[i].hasDeath) continue;
                notDeathFound = true;
                break;
            }

            if (notDeathFound) return;

            _mainConfigEntity.isGameOver = true;
        }
    }
}