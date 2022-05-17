using Entitas;

namespace GameEngine
{
    [Flight]
    [Environment]
    public sealed class MaxSpeedComponent : IComponent
    {
        public float Value;
    }
}