using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public sealed class InitializeConfigSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IRandomService _randomService;
        private readonly FlightData _flightData;
        private readonly TrapData _trapData;
        private readonly int _delay;

        public InitializeConfigSystem(Contexts contexts, Services services, Data data, int delay)
        {
            _contexts = contexts;
            _randomService = services.RandomService;
            _flightData = data.FlightData;
            _trapData = data.TrapData;
            _delay = delay;
        }

        public void Initialize()
        {
            var eMainConfig = _contexts.config.CreateEntity();

            eMainConfig.isMainConfig = true;
            {
                eMainConfig.AddPipeColorIndex(_randomService.Choose(0, 1, 0));
                eMainConfig.AddMinFlightSize(_flightData.MaxSizeInFlight);
                eMainConfig.AddLifeCountInStorage(_flightData.StartLifeInStorage);
                eMainConfig.AddFlightIndex(0);
                eMainConfig.AddDelayCount(_delay);
                eMainConfig.AddLevel(0, 0);
            }

            GameSettings.PlayCount = 0;
            if (GameSettings.IsDebugMode)
                eMainConfig.lifeCountInStorage.Value = 100;

            var eStatsConfig = _contexts.config.CreateEntity();
            eStatsConfig.isStatsConfig = true;
            {
                eStatsConfig.AddScore(0);
                eStatsConfig.AddStatsDiamond(0);
                eStatsConfig.AddStatsDead(0, new Dictionary<int, int>());
                eStatsConfig.AddStatsScoreSizeRace(0, 0);
            }

            foreach (var t in _trapData.Types)
                //if (!eStatsConfig.statsDead.CategoryCountGroupList.ContainsKey(t.GetInstanceID()))
                    eStatsConfig.statsDead.CategoryCountGroupList.Add(t.GetInstanceID(), 0);
        }
    }
}