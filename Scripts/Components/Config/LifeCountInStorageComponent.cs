using Entitas;

namespace GameEngine
{
    [Config]
    public sealed class LifeCountInStorageComponent : IComponent
    {
        public int Value;
    }
}