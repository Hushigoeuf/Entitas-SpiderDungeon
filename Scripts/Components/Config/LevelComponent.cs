using Entitas;

namespace GameEngine
{
    [Config]
    public sealed class LevelComponent : IComponent
    {
        public int FirstLevel;
        public int SecondLevel;

        public int FullLevel => FirstLevel + SecondLevel;
    }
}