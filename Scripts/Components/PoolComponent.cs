using Entitas;

namespace GameEngine
{
    [Environment]
    [Flight]
    public sealed class PoolComponent : IComponent
    {
        public string Value;
    }
}