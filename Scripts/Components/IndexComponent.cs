using Entitas;

namespace GameEngine
{
    [Environment]
    [Flight]
    public sealed class IndexComponent : IComponent
    {
        public int Value;
    }
}