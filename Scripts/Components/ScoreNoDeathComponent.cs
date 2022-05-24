using Entitas;

namespace GameEngine
{
    [Config]
    public class ScoreNoDeathComponent : IComponent
    {
        public int CurrentValue;
        public int MaxValue;
    }
}