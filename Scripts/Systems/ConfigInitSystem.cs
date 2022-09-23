using System.Collections.Generic;
using Entitas;

namespace GameEngine
{
    public class ConfigInitSystem : IInitializeSystem
    {
        private readonly Contexts _contexts;
        private readonly IRandomService _randomService;
        private readonly FlightSettings _flightSettings;
        private readonly TrapSettings _trapSettings;
        private readonly int _skipGenStepCount;

        public ConfigInitSystem(Contexts contexts, Services services, Settings settings, int skipGenStepCount)
        {
            _contexts = contexts;
            _randomService = services.Random;
            _flightSettings = settings.FlightSettings;
            _trapSettings = settings.TrapSettings;
            _skipGenStepCount = skipGenStepCount;
        }

        public void Initialize()
        {
            CreateMainConfig();
            var eStatConfig = CreateStatConfig();

            foreach (var t in _trapSettings.Types)
                eStatConfig.deathCount.CategoryCounts.Add(t.GetInstanceID(), 0);
        }

        private ConfigEntity CreateMainConfig()
        {
            var eMainConfig = _contexts.config.CreateEntity();
            eMainConfig.isMainConfig = true;
            {
                eMainConfig.AddPipeColorIndex(_randomService.Choose(0, 1, 0));
                eMainConfig.AddMinFlightSize(_flightSettings.MaxSizeInFlight);
                eMainConfig.AddLifeCountInStorage(_flightSettings.StartLifeInStorage);
                eMainConfig.AddFlightIndex(0);
                eMainConfig.AddDelayCount(_skipGenStepCount);
                eMainConfig.AddLevel(0, 0);
            }
            return eMainConfig;
        }

        private ConfigEntity CreateStatConfig()
        {
            var eStatConfig = _contexts.config.CreateEntity();
            eStatConfig.isStatConfig = true;
            {
                eStatConfig.AddScore(0);
                eStatConfig.AddDiamondCount(0);
                eStatConfig.AddDeathCount(0, new Dictionary<int, int>());
                eStatConfig.AddScoreNoDeath(0, 0);
            }
            return eStatConfig;
        }
    }
}