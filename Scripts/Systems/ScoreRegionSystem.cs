using Entitas;

namespace GameEngine
{
    /// <summary>
    /// Следит за игровым счетом и меняет уровень окружения по достижению целевых значений.
    /// </summary>
    public sealed class ScoreRegionSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly int[] _scoreRegions;

        private ConfigEntity _eMainConfig;
        private ConfigEntity _eStatConfig;
        private int _lastScore;

        public ScoreRegionSystem(Contexts contexts, Settings settings)
        {
            _contexts = contexts;
            _scoreRegions = settings.TrapSettings.ScoreRegions;
        }

        public void Initialize()
        {
            _eMainConfig = _contexts.config.mainConfigEntity;
            _eStatConfig = _contexts.config.statConfigEntity;
            _eStatConfig.OnComponentReplaced += OnComponentReplaced;
        }

        private void OnComponentReplaced(IEntity entity, int index, IComponent component, IComponent newComponent)
        {
            if (_eStatConfig.score.Value <= _lastScore) return;
            if (_eMainConfig.isGameOver) return;

            _lastScore = _eStatConfig.score.Value;

            var regionIndex = 0;
            for (var i = 0; i < _scoreRegions.Length; i++)
                if (_lastScore >= _scoreRegions[i]) regionIndex++;
                else break;

            if (_eMainConfig.level.FirstLevel != regionIndex)
                _eMainConfig.ReplaceLevel(regionIndex, _eMainConfig.level.SecondLevel);

            if (regionIndex >= _scoreRegions.Length)
                _eStatConfig.OnComponentReplaced -= OnComponentReplaced;
        }
    }
}