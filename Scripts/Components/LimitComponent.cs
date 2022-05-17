using Entitas;

namespace GameEngine
{
    [Flight]
    [Environment]
    public sealed class LimitComponent : IComponent
    {
        public float Value;
    }
}