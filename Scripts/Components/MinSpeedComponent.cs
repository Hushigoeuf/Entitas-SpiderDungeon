using Entitas;

namespace GameEngine
{
    [Flight]
    [Environment]
    public sealed class MinSpeedComponent : IComponent
    {
        public float Value;
    }
}