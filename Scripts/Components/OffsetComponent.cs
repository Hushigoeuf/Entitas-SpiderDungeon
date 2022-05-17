using Entitas;

namespace GameEngine
{
    [Flight]
    [Environment]
    public sealed class OffsetComponent : IComponent
    {
        public float Value;
    }
}