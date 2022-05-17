using Entitas;

namespace GameEngine
{
    public sealed class SaveStatsConfigSystem : IInitializeSystem, ITearDownSystem
    {
        private Contexts _contexts;
        private StatsData _statsData;
        private TrapData _trapData;

        private ConfigEntity _statsEntity;

        public SaveStatsConfigSystem(Contexts contexts, Data data)
        {
            _contexts = contexts;
            _statsData = data.StatsData;
            _trapData = data.TrapData;
        }

        public void Initialize()
        {
            _statsEntity = _contexts.config.statsConfigEntity;
        }

        public void TearDown()
        {
            // High Score
            if (_statsEntity.score.Value > _statsData.HighScoreParameter.Value)
            {
                _statsData.HighScoreParameter.Value = _statsEntity.score.Value;
                //Records Count
                _statsData.RecordsCountParameter.Value++;
            }

            // First Score
            if (!_statsData.FirstScoreParameter.IsExists)
                _statsData.FirstScoreParameter.Value = _statsEntity.score.Value;
            // Last Score
            _statsData.LastScoreParameter.Value = _statsEntity.score.Value;

            // Diamond Count
            _statsData.DiamondParameter.Value += _statsEntity.statsDiamond.Value;
            // All Found Diamond Count
            _statsData.DiamondFoundParameter.Value += _statsEntity.statsDiamond.Value;

            // Dead Count
            _statsData.DeathsCountParameter.Value += _statsEntity.statsDead.NoCategoryCount;
            foreach (var t in _trapData.Types)
            {
                var c = _statsEntity.statsDead.CategoryCountGroupList[t.GetInstanceID()];
                t.DeadCounterParameter.Value += c;
                _statsData.DeathsCountParameter.Value += c;
            }
            
            // Raid COunt
            _statsData.RaidCountParameter.Value++;

            if (_statsEntity.statsScoreSizeRace.MaxValue > _statsData.LongestScoreRaceParameter.Value)
                _statsData.LongestScoreRaceParameter.Value = _statsEntity.statsScoreSizeRace.MaxValue;
        }
    }
}