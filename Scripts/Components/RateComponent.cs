using Entitas;

namespace GameEngine
{
    [Flight]
    [Environment]
    public sealed class RateComponent : IComponent
    {
        public float Value;
    }
}