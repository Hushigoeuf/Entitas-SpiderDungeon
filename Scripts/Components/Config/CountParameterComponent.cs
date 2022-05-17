using Entitas;

namespace GameEngine
{
    [Config]
    public sealed class CountParameterComponent : IComponent
    {
        public CountParameterObject Value;
    }
}