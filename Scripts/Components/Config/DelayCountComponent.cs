using Entitas;

namespace GameEngine
{
    [Config]
    public sealed class DelayCountComponent : IComponent
    {
        public int Value;
    }
}