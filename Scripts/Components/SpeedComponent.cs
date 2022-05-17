using Entitas;

namespace GameEngine
{
    [Flight]
    [Environment]
    public sealed class SpeedComponent : IComponent
    {
        public float Value;
    }
}