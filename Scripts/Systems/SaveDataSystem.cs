using Entitas;

namespace GameEngine
{
    public class SaveDataSystem : IInitializeSystem, ITearDownSystem
    {
        private Contexts _contexts;
        private StatSettings _statSettings;
        private TrapSettings _trapSettings;

        private ConfigEntity _eStatConfig;

        public SaveDataSystem(Contexts contexts, Settings settings)
        {
            _contexts = contexts;
            _statSettings = settings.StatSettings;
            _trapSettings = settings.TrapSettings;
        }

        public void Initialize()
        {
            _eStatConfig = _contexts.config.statConfigEntity;
        }

        public void TearDown()
        {
            if (_eStatConfig.score.Value > _statSettings.HighScoreParameter.Value)
            {
                _statSettings.HighScoreParameter.Value = _eStatConfig.score.Value;
                _statSettings.RecordCountParameter.Value++;
            }

            if (!_statSettings.FirstScoreParameter.IsExists)
                _statSettings.FirstScoreParameter.Value = _eStatConfig.score.Value;
            _statSettings.LastScoreParameter.Value = _eStatConfig.score.Value;

            _statSettings.DiamondParameter.Value += _eStatConfig.diamondCount.Value;
            _statSettings.DiamondFoundParameter.Value += _eStatConfig.diamondCount.Value;

            _statSettings.DeathCountParameter.Value += _eStatConfig.deathCount.NoCategoryCount;
            foreach (var t in _trapSettings.Types)
            {
                int c = _eStatConfig.deathCount.CategoryCounts[t.GetInstanceID()];
                t.DeathCounter.Value += c;
                _statSettings.DeathCountParameter.Value += c;
            }

            _statSettings.PlayCountParameter.Value++;

            if (_eStatConfig.scoreNoDeath.MaxValue > _statSettings.LongestScoreRaceParameter.Value)
                _statSettings.LongestScoreRaceParameter.Value = _eStatConfig.scoreNoDeath.MaxValue;
        }
    }
}