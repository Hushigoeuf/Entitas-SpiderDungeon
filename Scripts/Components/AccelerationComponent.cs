using Entitas;

namespace GameEngine
{
    [Flight]
    [Environment]
    public sealed class AccelerationComponent : IComponent
    {
        public float Value;
    }
}