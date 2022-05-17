using Entitas;

namespace GameEngine
{
    public sealed class RegionConfigSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly int[] _scoreRegions;
        private readonly SoundManager _soundManager;

        private ConfigEntity _mainConfigEntity;
        private ConfigEntity _statsEntity;
        private int _lastScore;

        public RegionConfigSystem(Contexts contexts, Data data, SoundManager soundManager)
        {
            _contexts = contexts;
            _scoreRegions = data.TrapData.ScoreRegions;
            _soundManager = soundManager;

            if (GameSettings.IsDebugMode)
            {
                _scoreRegions = new int[_scoreRegions.Length];
                for (var i = 0; i < _scoreRegions.Length; i++)
                    _scoreRegions[i] = 50 * i + 50;
            }
        }

        public void Initialize()
        {
            _mainConfigEntity = _contexts.config.mainConfigEntity;
            _statsEntity = _contexts.config.statsConfigEntity;
            _statsEntity.OnComponentReplaced += _OnScoreReplaced;
        }

        private void _OnScoreReplaced(IEntity entity, int index, IComponent component, IComponent newComponent)
        {
            if (_statsEntity.score.Value <= _lastScore) return;
            if (_mainConfigEntity.isGameOver) return;

            _lastScore = _statsEntity.score.Value;

            var regionIndex = 0;
            for (var i = 0; i < _scoreRegions.Length; i++)
                if (_lastScore >= _scoreRegions[i]) regionIndex++;
                else break;

            if (_mainConfigEntity.level.FirstLevel != regionIndex)
            {
                _mainConfigEntity.ReplaceLevel(regionIndex, _mainConfigEntity.level.SecondLevel);
                _soundManager?.ForceChangeMusic(1);
            }

            if (regionIndex >= _scoreRegions.Length)
                _statsEntity.OnComponentReplaced -= _OnScoreReplaced;
        }
    }
}