using Entitas;

namespace GameEngine
{
    [Config]
    public sealed class StatsScoreSizeRaceComponent : IComponent
    {
        public int CurrentValue;
        public int MaxValue;
    }
}